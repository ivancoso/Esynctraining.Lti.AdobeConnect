//
//  MethodInvocationViewController.h
//  MessagingApps
//
//  Created by Vyacheslav Vdovichenko on 8/1/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "RTMPClient.h"

@interface MethodInvocationViewController : UIViewController <UITextFieldDelegate, UIWebViewDelegate, IRTMPClientDelegate> {
    
	RTMPClient	*socket;
	int			alerts;
	
	IBOutlet UITextField    *hostTextField;
    IBOutlet UIWebView      *helpWebView;
	IBOutlet UITextView		*noteTextView;
    IBOutlet UIBarButtonItem *btnConnect;
	IBOutlet UIButton       *btnEchoInt;
	IBOutlet UIButton       *btnEchoFloat;
	IBOutlet UIButton       *btnEchoString;
	IBOutlet UIButton       *btnEchoStringArray;
    
    UIActivityIndicatorView *netActivity;    
}

-(IBAction)connectControl:(id)sender;
-(IBAction)echoInt:(id)sender;
-(IBAction)echoFloat:(id)sender;
-(IBAction)echoString:(id)sender;
-(IBAction)echoStringArray:(id)sender;

@end
