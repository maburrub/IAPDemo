#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// You must obfuscate your secrets using Window > Unity IAP > Receipt Validation Obfuscator
// before receipt validation will compile in this sample.
// #define RECEIPT_VALIDATION
#endif

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;
#if RECEIPT_VALIDATION
using UnityEngine.Purchasing.Security;
#endif

/// <summary>
/// An example of basic Unity IAP functionality.
/// To use with your account, configure the product ids (AddProduct)
/// and Google Play key (SetPublicKey).
/// </summary>
[AddComponentMenu("Unity IAP/Demo")]
public class IAPDemo : MonoBehaviour, IStoreListener
{
	// Unity IAP objects 
	private IStoreController m_Controller;
	private IAppleExtensions m_AppleExtensions;
	private IMoolahExtension m_MoolahExtensions;


	private int m_SelectedItemIndex = -1; // -1 == no product
	private bool m_PurchaseInProgress;

	// For Cloud Moolah Platform.
	private bool m_IsCloudMoolahStoreSelected; 
	private string m_CloudMoolahTransationID;
	private string m_CloudMoolahReceipt;
	private string m_CloudMoolahUserName;

	private Selectable m_InteractableSelectable; // Optimization used for UI state management

	#if RECEIPT_VALIDATION
	private CrossPlatformValidator validator;
	#endif

	/// <summary>
	/// This will be called when Unity IAP has finished initialising.
	/// </summary>
	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		m_Controller = controller;
		m_AppleExtensions = extensions.GetExtension<IAppleExtensions> ();
		m_MoolahExtensions = extensions.GetExtension<IMoolahExtension> ();

		InitUI(controller.products.all);

		// On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.
		// On non-Apple platforms this will have no effect; OnDeferred will never be called.
		m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);

		Debug.Log("Available items:");
		foreach (var item in controller.products.all)
		{
			if (item.availableToPurchase)
			{
				Debug.Log(string.Join(" - ",
					new[]
					{
						item.metadata.localizedTitle,
						item.metadata.localizedDescription,
						item.metadata.isoCurrencyCode,
						item.metadata.localizedPrice.ToString(),
						item.metadata.localizedPriceString,
						item.transactionID,
						item.receipt
					}));
			}
		}

		// Prepare model for purchasing
		if (m_Controller.products.all.Length > 0) 
		{
			m_SelectedItemIndex = 0;
		}

		// Populate the product menu now that we have Products
		for (int t = 0; t < m_Controller.products.all.Length; t++)
		{
			var item = m_Controller.products.all[t];
			var description = string.Format("{0} - {1}", item.metadata.localizedTitle, item.metadata.localizedPriceString);

			// NOTE: my options list is created in InitUI
			GetDropdown().options[t] = new Dropdown.OptionData(description);
		}

		// Ensure I render the selected list element
		GetDropdown().RefreshShownValue();

		// Now that I have real products, begin showing product purchase history
		UpdateHistoryUI();
	}
	/// <summary>
	/// This will be called when a purchase completes.
	/// </summary>
	/// 
	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		Debug.Log("Purchase OK: " + e.purchasedProduct.definition.id);
		Debug.Log("Receipt: " + e.purchasedProduct.receipt);
		m_CloudMoolahTransationID = e.purchasedProduct.transactionID;
		m_CloudMoolahReceipt =  e.purchasedProduct.receipt;
		m_PurchaseInProgress = false;

		// Now that my purchase history has changed, update its UI
		UpdateHistoryUI();

		#if RECEIPT_VALIDATION
		if (Application.platform == RuntimePlatform.Android ||
			Application.platform == RuntimePlatform.IPhonePlayer ||
			Application.platform == RuntimePlatform.OSXPlayer) {
			try {
				var result = validator.Validate(e.purchasedProduct.receipt);
				Debug.Log("Receipt is valid. Contents:");
				foreach (IPurchaseReceipt productReceipt in result) {
					Debug.Log(productReceipt.productID);
					Debug.Log(productReceipt.purchaseDate);
					Debug.Log(productReceipt.transactionID);

					GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
					if (null != google) {
						Debug.Log(google.purchaseState);
						Debug.Log(google.purchaseToken);
					}

					AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
					if (null != apple) {
						Debug.Log(apple.originalTransactionIdentifier);
						Debug.Log(apple.subscriptionExpirationDate);
						Debug.Log(apple.cancellationDate);
						Debug.Log(apple.quantity);
					}
				}
			} catch (IAPSecurityException) {
				Debug.Log("Invalid receipt, not unlocking content");
				return PurchaseProcessingResult.Complete;
			}
		}
		#endif

		// You should unlock the content here.

		// Indicate we have handled this purchase, we will not be informed of it again.x
		return PurchaseProcessingResult.Complete;
	}

	/// <summary>
	/// This will be called is an attempted purchase fails.
	/// </summary>
	public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
	{
		Debug.Log("Purchase failed: " + item.definition.id);
		Debug.Log(r);
		m_PurchaseInProgress = false;
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.Log("Billing failed to initialize!");
		switch (error)
		{
			case InitializationFailureReason.AppNotKnown:
				Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
				break;
			case InitializationFailureReason.PurchasingUnavailable:
				// Ask the user if billing is disabled in device settings.
				Debug.Log("Billing disabled!");
				break;
			case InitializationFailureReason.NoProductsAvailable:
				// Developer configuration error; check product metadata.
				Debug.Log("No products available for purchase!");
				break;
		}
	}



	public void Awake()
	{
		var module = StandardPurchasingModule.Instance();

		// The FakeStore supports: no-ui (always succeeding), basic ui (purchase pass/fail), and 
		// developer ui (initialization, purchase, failure code setting). These correspond to 
		// the FakeStoreUIMode Enum values passed into StandardPurchasingModule.useFakeStoreUIMode.
		module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;

		var builder = ConfigurationBuilder.Instance(module);

		// This records whether we are using Cloud Moolah IAP. 
		// Cloud Moolah requires logging in, so: 
		// A) IAPDemo (this) displays the Cloud Moolah GUI button for Cloud Moolah
		m_IsCloudMoolahStoreSelected = Application.platform == RuntimePlatform.Android  && module.androidStore == AndroidStore.CloudMoolah;


		// This enables the Microsoft IAP simulator for local testing.
		// You would remove this before building your release package.
		builder.Configure<IMicrosoftConfiguration>().useMockBillingSystem = true;
		builder.Configure<IGooglePlayConfiguration>().SetPublicKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA2O/9/H7jYjOsLFT/uSy3ZEk5KaNg1xx60RN7yWJaoQZ7qMeLy4hsVB3IpgMXgiYFiKELkBaUEkObiPDlCxcHnWVlhnzJBvTfeCPrYNVOOSJFZrXdotp5L0iS2NVHjnllM+HA1M0W2eSNjdYzdLmZl1bxTpXa4th+dVli9lZu7B7C2ly79i/hGTmvaClzPBNyX+Rtj7Bmo336zh2lYbRdpD5glozUq+10u91PMDPH+jqhx10eyZpiapr8dFqXl5diMiobknw9CgcjxqMTVBQHK6hS0qYKPmUDONquJn280fBs1PTeA6NMG03gb9FLESKFclcuEZtvM8ZwMMRxSLA9GwIDAQAB");

		// cloudmoolah data setings for test
		// All games must set the configuration. the configuration need to apply in the cloudmoolah Platform.
		// manufacturer service identifer
		builder.Configure<IMoolahConfiguration>().appKey = "54ad14a8a350e0a71d4764ae9825fc0e";
		// for purchase 
		builder.Configure<IMoolahConfiguration>().hashKey = "asdfasdfadsfsadcok-test-a-d";



		// Define our products.
		// In this case our products have the same identifier across all the App stores,
		// except on the Mac App store where product IDs cannot be reused across both Mac and
		// iOS stores.
		// So on the Mac App store our products have different identifiers,
		// and we tell Unity IAP this by using the IDs class.


		builder.AddProduct("100.gold.coins", ProductType.Consumable, new IDs
			{
				{"100.gold.coins.mac", MacAppStore.Name},
			});

		builder.AddProduct("500.gold.coins", ProductType.Consumable, new IDs
			{
				{"500.gold.coins.mac", MacAppStore.Name},
			});

		builder.AddProduct("sword", ProductType.NonConsumable, new IDs
			{
				{"sword.mac", MacAppStore.Name}
			});
		builder.AddProduct("subscription", ProductType.Subscription, new IDs
			{
				{"subscription.mac", MacAppStore.Name}
			});
		
		// Define Cloud Moolah Product, it is available for test on Cloud Moolah Android Platform.
		builder.AddProduct("100.gold.coins.cloudmoolah", ProductType.Consumable);
		
		// Write Amazon's JSON description of our products to storage when using Amazon's local sandbox.
		// This should be removed from a production build.
		builder.Configure<IAmazonConfiguration>().WriteSandboxJSON(builder.products);

		#if RECEIPT_VALIDATION
		validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.bundleIdentifier);
		#endif

		// Now we're ready to initialize Unity IAP.
		UnityPurchasing.Initialize(this, builder);

		//Debug.Log ("cash Path:" + Application.persistentDataPath);
	}

	/// <summary>
	/// This will be called after a call to IAppleExtensions.RestoreTransactions().
	/// </summary>
	private void OnTransactionsRestored(bool success)
	{
		Debug.Log("Transactions restored.");
	}

	/// <summary>
	/// iOS Specific.
	/// This is called as part of Apple's 'Ask to buy' functionality,
	/// when a purchase is requested by a minor and referred to a parent
	/// for approval.
	/// 
	/// When the purchase is approved or rejected, the normal purchase events
	/// will fire.
	/// </summary>
	/// <param name="item">Item.</param>
	private void OnDeferred(Product item)
	{
		Debug.Log("Purchase deferred: " + item.definition.id);
	}

	private void InitUI(IEnumerable<Product> items)
	{
		// Disable the UI while IAP is initializing
		// See also UpdateInteractable()
		m_InteractableSelectable = GetDropdown(); // References any one of the disabled components

		// Show Restore button on Apple platforms
		if (Application.platform != RuntimePlatform.IPhonePlayer && 
			Application.platform != RuntimePlatform.OSXPlayer)
		{
			GetRestoreButton().gameObject.SetActive(false);
		}
		// show button on Cloud Moolah platforms, or hide button on others.
		GetLoginButton ().gameObject.SetActive (m_IsCloudMoolahStoreSelected);
		GetRegisternButton ().gameObject.SetActive (m_IsCloudMoolahStoreSelected);
		GetCMRestoreButton ().gameObject.SetActive (m_IsCloudMoolahStoreSelected);
		GetValidButton ().gameObject.SetActive (m_IsCloudMoolahStoreSelected);
		GetPayoutButton ().gameObject.SetActive (m_IsCloudMoolahStoreSelected);

		foreach (var item in items)
		{
			// Add initial pre-IAP-initialization content. Update later in OnInitialized.
			var description = string.Format("{0} - {1}", item.definition.id, item.definition.type);

			GetDropdown().options.Add(new Dropdown.OptionData(description));
		}

		// Ensure I render the selected list element
		GetDropdown().RefreshShownValue();

		GetDropdown().onValueChanged.AddListener((int selectedItem) => {
			Debug.Log("OnClickDropdown item " + selectedItem);
			m_SelectedItemIndex = selectedItem;
		});

		// Initialize my button event handling
		GetBuyButton().onClick.AddListener(() => { 
			if (m_PurchaseInProgress == true) {
				return;
			}
			// Online Game need to login on Cloud Moolah PlatForm.
			// Stand-alone Game may not need to  login.
			// our Server need to notice GameServer, So online Game need to login on Cloud Moolah PlatForm.
			if( m_MoolahExtensions.isLogined==false && m_IsCloudMoolahStoreSelected)
			{
				Debug.LogError("Cloud Moolah need to login!");
				return;
			}
			m_Controller.InitiatePurchase(m_Controller.products.all[m_SelectedItemIndex]); 

			// Don't need to draw our UI whilst a purchase is in progress.
			// This is not a requirement for IAP Applications but makes the demo
			// scene tidier whilst the fake purchase dialog is showing.
			m_PurchaseInProgress = true;
		});

		if (GetRestoreButton() != null)
		{
			GetRestoreButton().onClick.AddListener(() =>
			{ 
				m_AppleExtensions.RestoreTransactions(OnTransactionsRestored);
			});
		}
		//-----CLoud Moolah Extensions Example-----
		//-----start-----
		if(GetLoginButton()	!= null)
		{
			GetLoginButton ().onClick.AddListener (() => {
				m_MoolahExtensions.login (m_CloudMoolahUserName, "CMPassword", loginSuccessed,loginFailed);
			});
		}
		if(GetRegisternButton()	!= null)
		{
			GetRegisternButton ().onClick.AddListener (() => {
				//Use Game User ID and password　register a Cloud Moolah Acount.
				m_MoolahExtensions.registerOrBinding ("GameUserID", "CMPassword",registerOrBindingSuccessed, registerOrBindingFailed);
			});
		}
		if(GetCMRestoreButton() != null)
		{
			GetCMRestoreButton ().onClick.AddListener (() => {
				// Online Game need to login on Cloud Moolah PlatForm.
				// Stand-alone Game may not need to  login.
				// Restore TransactionID need to login on online Game.   
				if(m_MoolahExtensions.isLogined==false && m_IsCloudMoolahStoreSelected)
				{
					Debug.LogError("Cloud Moolah need to login!");
					return;
				}
				// Restore abnornal transaction identifer, if Client don't receive transaction identifer.
				m_MoolahExtensions.restoreTransactionID ((RestoreTransactionIDState restoreTransactionIDState)=>{
					Debug.Log("RestoreTransactionIDState is " + restoreTransactionIDState.ToString());
				});
			});
		}
		if(GetPayoutButton() != null)
		{
			GetPayoutButton ().onClick.AddListener (() => {
				//Stand-alone Game need to Payout, change transaction identifer state.
				m_MoolahExtensions.requestPayOut(m_CloudMoolahTransationID, (string transationID,RequestPayOutState state, string msg)=>{
					Debug.Log("requestPayOut TannsationID:" + transationID + ",state:"+ state.ToString() + ",msg:"+msg);
				} );
			});
		}
		if(GetValidButton() != null)
		{
			GetValidButton ().onClick.AddListener (() => {
				//vaildate receipt true or false.
				m_MoolahExtensions.validateReceipt (m_CloudMoolahTransationID,m_CloudMoolahReceipt, (string transationID,ValidateReceiptState state, string  msg)=>{
					Debug.Log("validateReceipt TannsationID:" + transationID + ",state:"+ state.ToString() + ",msg:"+msg);
				});
			});
		}
		//-----End-----

	}
	//---------------------login and registerOrBinding callBack--------------------------
	//-----------------------------start------------------------------------	
	public void loginSuccessed (string token,string CMuserid,string CMAcount)
	{
		Debug.Log ("loginSuccessed: CMuserid is " + CMuserid);
	}
	public void loginFailed (LoginError error, string errorMsg)
	{
		Debug.Log ("loginFailed: type is " + error.ToString () + " ! msg is " + errorMsg);
	}
	public void registerOrBindingSuccessed (string cmUserName, string cmID)
	{
		m_CloudMoolahUserName = cmUserName;
		Debug.Log ("registerOrBindingSuccessed : cmUserName is " + cmUserName + " , cmID is " + cmID);
	}
	public void registerOrBindingFailed (RegisterOrBindingError error, string errorMsg)
	{
		Debug.Log ("registerOrBindingFailed :RegisterOrBindingError is " + error.ToString() + ", errorMsg is " + errorMsg);
	}
	//----------------------------end---------------------------------------	
	public void UpdateHistoryUI()
	{
		if (m_Controller == null)
		{
			return;
		}

		var itemText = "Item\n\n";
		var countText = "Purchased\n\n";

		for (int t = 0; t < m_Controller.products.all.Length; t++)
		{
			var item = m_Controller.products.all [t];

			// Collect history status report

			itemText += "\n\n" + item.definition.id;
			countText += "\n\n" + item.hasReceipt.ToString();
		}

		// Show history
		GetText(false).text = itemText;
		GetText(true).text = countText;
	}

	protected void UpdateInteractable()
	{
		if (m_InteractableSelectable == null)
		{
			return;
		}

		bool interactable = m_Controller != null;
		if (interactable != m_InteractableSelectable.interactable)
		{
			if (GetRestoreButton() != null)
			{
				GetRestoreButton().interactable = interactable;
			}
			GetBuyButton().interactable = interactable;
			GetDropdown().interactable = interactable;
			GetLoginButton().interactable = interactable;
			GetRegisternButton().interactable = interactable;
		}
	}

	public void Update()
	{
		UpdateInteractable();
	}

	private Dropdown GetDropdown()
	{
		return GameObject.Find("Dropdown").GetComponent<Dropdown>();
	}

	private Button GetBuyButton()
	{
		return GameObject.Find("Buy").GetComponent<Button>();
	}

	/// <summary>
	/// Gets the restore button when available
	/// </summary>
	/// <returns><c>null</c> or the restore button.</returns>
	private Button GetRestoreButton()
	{
		return GetButton ("Restore");
	}
	// Cloud Moolah Platform Button
	//------start------
	private Button GetLoginButton()
	{
		return GetButton ("Login");
	}
	private Button GetRegisternButton()
	{
		return GetButton ("Register");
	}
	private Button GetPayoutButton()
	{
		return GetButton ("PayOut");
	}
	private Button GetCMRestoreButton()
	{
		return GetButton ("cmRestore");
	}
	private Button GetValidButton()
	{
		return GetButton ("Valid");
	}
	//------end------
	private  Button GetButton(string buttonName)
	{
		GameObject obj = GameObject.Find(buttonName);
		if (obj != null)
		{
			return obj.GetComponent <Button>();
		}
		else
		{
			return null;
		}
	}
	private Text GetText(bool right)
	{
		var which = right ? "TextR" : "TextL";
		return GameObject.Find(which).GetComponent<Text>();
	}
}
