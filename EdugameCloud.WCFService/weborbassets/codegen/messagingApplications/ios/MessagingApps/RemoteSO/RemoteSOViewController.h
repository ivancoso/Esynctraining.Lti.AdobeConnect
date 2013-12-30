//
//  RemoteSOViewController.h
//  MessagingApps
//
//  Created by Vyacheslav Vdovichenko on 8/1/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "RTMPClient.h"
#import "ISharedObjectListener.h"

@interface RemoteSOViewController : UIViewController <UITextFieldDelegate, IRTMPClientDelegate, ISharedObjectListener> {
    
	RTMPClient	*socket;
	int			alerts;
    
    id <IClientSharedObject>  clientSO;
    int         intSO;
    float       floatSO;
	
	IBOutlet UITextField    *hostTextField;
	IBOutlet UITextView		*noteTextView;
    IBOutlet UIBarButtonItem *btnConnect;
	IBOutlet UIButton       *btnConnectSO;
	IBOutlet UIButton       *btnGetAttributeSO;
	IBOutlet UIButton       *btnSendMessageSO;
	IBOutlet UIButton       *btnRemoveAttributeSO;
    
    UIActivityIndicatorView *netActivity;
}

-(IBAction)connectControl:(id)sender;
-(IBAction)connectSO:(id)sender;
-(IBAction)getAttributeSO:(id)sender;
-(IBAction)sendMessageSO:(id)sender;
-(IBAction)removeAttributeSO:(id)sender;

@end
