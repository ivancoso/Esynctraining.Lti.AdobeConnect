//
//  AppDelegate.h
//  MessagingDest
//
//  Created by Vyacheslav Vdovichenko on 7/27/12.
//  Copyright (c) 2012 The Midnight Coders, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface AppDelegate : UIResponder <UIApplicationDelegate>

@property (strong, nonatomic) UIWindow *window;

+(NSString *)defaultURL;
+(NSString *)defaultDestinationName;

@end
