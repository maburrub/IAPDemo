#import <StoreKit/StoreKit.h>

@interface UnityTrouble : NSObject <SKPaymentTransactionObserver> {
}
@end

@implementation UnityTrouble
// The transaction status of the SKPaymentQueue is sent here.
- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions {
    for(SKPaymentTransaction *transaction in transactions) {
        
        switch (transaction.transactionState) {
                
            case SKPaymentTransactionStatePurchasing:
                NSLog(@"Snooped purchasing transaction");
                break;
                
            case SKPaymentTransactionStatePurchased:
            case SKPaymentTransactionStateRestored: {
                NSLog(@"Snooped purchased transaction");
                break;
            }
            case SKPaymentTransactionStateDeferred:
                break;
            case SKPaymentTransactionStateFailed: {
                NSLog(@"Snooped failed transaction");
            }
                break;
        }
    }
}
@end

// prototype
void myStaticInitMethod(void);
UnityTrouble* instance;

__attribute__((constructor))
void myStaticInitMethod()
{
    // code here will be called as soon as the binary is loaded into memory
    // before any other code has a chance to call +initialize.
    // useful for a situation where you have a struct that must be
    // initialized before any calls are made to the class,
    // as they would be used as parameters to the constructors.
    // e.g.
    instance = [[UnityTrouble alloc] init];
    [[SKPaymentQueue defaultQueue] addTransactionObserver:instance];
    
    // so when the user calls the code [MyClass createClassFromStruct:myStructDef],
    // myStructDef is not junk values.
}