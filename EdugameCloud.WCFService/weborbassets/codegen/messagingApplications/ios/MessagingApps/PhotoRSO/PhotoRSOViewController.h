//
//  PhotoRSOViewController.h
//  MessagingApps
//
//  Created by Vyacheslav Vdovichenko on 8/2/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <AVFoundation/AVFoundation.h>
#import "RTMPClient.h"
#import "ISharedObjectListener.h"

@interface PhotoRSOViewController : UIViewController <UITextFieldDelegate,  IRTMPClientDelegate, ISharedObjectListener> {
    
    AVCaptureSession            *session;
	AVCaptureVideoDataOutput    *videoDataOutput;
	AVCaptureVideoPreviewLayer  *previewLayer;
	dispatch_queue_t            videoDataOutputQueue;
	AVCaptureStillImageOutput   *stillImageOutput;
	UIView                      *flashView;
	BOOL                        isUsingFrontFacingCamera;
    BOOL                        isPhotoPicking;
    
	RTMPClient                  *socket;
    id <IClientSharedObject>    clientSO;
	
    int                         alerts;
    
    IBOutlet UITextField        *hostTextField;
    IBOutlet UITextField        *nameTextField;
    IBOutlet UILabel            *saveAlbumLabel;
    IBOutlet UISwitch           *saveAlbumSwitch;
    IBOutlet UIView             *previewView;
    IBOutlet UIImageView        *photoView;
    IBOutlet UIBarButtonItem    *btnConnect;
    IBOutlet UIBarButtonItem    *btnToggleCameras;
    IBOutlet UIBarButtonItem    *btnToggleViews;
    IBOutlet UIBarButtonItem    *btnPhoto;
    
}

-(IBAction)connectControl:(id)sender;
-(IBAction)toggleCameras:(id)sender;
-(IBAction)toggleViews:(id)sender;
-(IBAction)photoControl:(id)sender;



@end
