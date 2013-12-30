//
//  DataPushViewController.h
//  MessagingApps
//
//  Created by Vyacheslav Vdovichenko on 8/1/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "RTMPClient.h"

@interface DataPushViewController : UIViewController <UITextFieldDelegate, UIWebViewDelegate, IRTMPClientDelegate> {
    
	RTMPClient	*socket;
	int			alerts;
	
	IBOutlet UITextField    *hostTextField;
    IBOutlet UIWebView      *helpWebView;
	IBOutlet UITextView		*noteTextView;
    IBOutlet UIBarButtonItem *btnConnect;
    
    UIActivityIndicatorView *netActivity;
}

-(IBAction)connectControl:(id)sender;

@end
