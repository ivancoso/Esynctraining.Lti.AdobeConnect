//
//  ViewController.h
//  MessagingDest
//
//  Created by Vyacheslav Vdovichenko on 7/26/12.
//  Copyright (c) The Midnight Coders, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "MessagingClient.h"

@interface ViewController : UIViewController <UITextFieldDelegate> {
    
    MessagingClient *client;
    int             alerts;
    
    IBOutlet UITextField    *hostTextField;
    IBOutlet UITextField    *destTextField;
    IBOutlet UIButton       *btnAccept;
    IBOutlet UILabel        *clientIdLabel;
    IBOutlet UITextField    *clientIdTextField;
    IBOutlet UITextField    *chatTextField;
    IBOutlet UITextView     *chatTextView;
    IBOutlet UIToolbar      *chatToolBar;
    
}

-(IBAction)doSettings:(id)sender;
-(IBAction)doAccept:(id)sender;
-(IBAction)doSubscribe:(id)sender;
-(IBAction)doUnsubscribe:(id)sender;

@end
