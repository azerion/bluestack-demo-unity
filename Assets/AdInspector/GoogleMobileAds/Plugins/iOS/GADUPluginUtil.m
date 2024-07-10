#import "GADUPluginUtil.h"

@interface UIView (unityStub)
@property UILayoutGuide *safeAreaLayoutGuide;
@end

BOOL GADUIsOperatingSystemAtLeastVersion(NSInteger majorVersion) {
  NSProcessInfo *processInfo = NSProcessInfo.processInfo;
  if ([processInfo respondsToSelector:@selector(isOperatingSystemAtLeastVersion:)]) {
    // iOS 8+.
    NSOperatingSystemVersion version = {majorVersion};
    return [processInfo isOperatingSystemAtLeastVersion:version];
  } else {
    // pre-iOS 8. App supports iOS 7+, so this process must be running on iOS 7.
    return majorVersion >= 7;
  }
}

@implementation GADUPluginUtil

+ (NSString *)GADUStringFromUTF8String:(const char *)bytes {
  return bytes ? @(bytes) : nil;
}

+ (UIViewController *)unityGLViewController {
  id<UIApplicationDelegate> appDelegate = [UIApplication sharedApplication].delegate;
  if ([appDelegate respondsToSelector:@selector(rootViewController)]) {
    return [[[UIApplication sharedApplication].delegate window] rootViewController];
  }
  return nil;
}

@end
