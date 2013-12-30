//
//  VideoPlayerViewController.h
//  MessagingApps
//
//  Created by Vyacheslav Vdovichenko on 8/1/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "MediaStreamPlayer.h"

@interface VideoPlayerViewController : UIViewController <UITextFieldDelegate, IMediaStreamEvent> {
    
    MediaStreamPlayer       *player;
    
	IBOutlet UITextField	*hostTextField;
	IBOutlet UITextField	*streamTextField;
    IBOutlet UIImageView    *previewView;
    IBOutlet UIBarButtonItem *btnConnect;
    IBOutlet UIBarButtonItem *btnPlay;
    
}

-(IBAction)connectControl:(id)sender;
-(IBAction)playControl:(id)sender;

@end
