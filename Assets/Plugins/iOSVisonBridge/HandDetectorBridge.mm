#import <Foundation/Foundation.h>
#import "UnityXRNativePtrs.h"
#import "UnityAppController.h"
#import <ARKit/ARKit.h>
#import <Metal/Metal.h>
#import <CoreVideo/CVPixelBuffer.h>
#include "Unity/IUnityInterface.h"
#include "UnityFramework/UnityFramework-Swift.h"

extern "C" {
    //static int r = 0;
    static HandDetector *handGestureProcessor = nil;

    void IOSVisonBridgeInit()
    {
        NSLog(@"HandGestureProcessor Init");
        handGestureProcessor = [[HandDetector alloc]init];
    }

    char *  IOSVisonBridgeDetectHand (intptr_t ptr)
    {
        if (handGestureProcessor==nil) {
            IOSVisonBridgeInit();
        }
        NSString* res = @"";
        if (ptr){
            UnityXRNativeFrame_1* unityXRFrame = (UnityXRNativeFrame_1*) ptr;
            ARFrame* frame = (__bridge ARFrame*)unityXRFrame->framePtr;
            CVPixelBufferRef buffer = frame.capturedImage;
            res = [[HandDetector shared] DetectHandWithBuffer: buffer];
        }
        
        return strdup([res UTF8String]);
    }

    char *  IOSVisonBridgeDetectMultiHand (intptr_t ptr,NSInteger maxHandCount)
    {
        if (handGestureProcessor==nil) {
            IOSVisonBridgeInit();
        }
        NSString* res = @"";
        if (ptr){
            UnityXRNativeFrame_1* unityXRFrame = (UnityXRNativeFrame_1*) ptr;
            ARFrame* frame = (__bridge ARFrame*)unityXRFrame->framePtr;
            CVPixelBufferRef buffer = frame.capturedImage;
            res = [[HandDetector shared] DetectMultiHandWithBuffer:(CVPixelBufferRef _Nonnull) buffer maximumHandCount:(NSInteger)maxHandCount];
        }
        
        return strdup([res UTF8String]);
    }
}


