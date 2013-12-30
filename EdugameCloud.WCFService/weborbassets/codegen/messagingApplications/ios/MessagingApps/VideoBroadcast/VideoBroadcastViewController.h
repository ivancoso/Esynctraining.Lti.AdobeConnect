//
//  VideoBroadcastViewController.h
//  MessagingApps
//
//  Created by Vyacheslav Vdovichenko on 8/1/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "BroadcastStreamClient.h"

@interface VideoBroadcastViewController : UIViewController <UITextFieldDelegate, IMediaStreamEvent> {
    
    BroadcastStreamClient   *upstream;
    
	IBOutlet UITextField	*hostTextField;
	IBOutlet UITextField	*streamTextField;
    IBOutlet UIView         *previewView;
    IBOutlet UIBarButtonItem *btnConnect;
    IBOutlet UIBarButtonItem *btnToggle;
    IBOutlet UIBarButtonItem *btnPublish;
    
}

-(IBAction)connectControl:(id)sender;
-(IBAction)publishControl:(id)sender;
-(IBAction)camerasToggle:(id)sender;

@end
