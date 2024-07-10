#import <GoogleMobileAds/GoogleMobileAds.h>
#import "GADURequestConfiguration.h"
#import "GADUPluginUtil.h"
#import "GADUObjectCache.h"
#import "GADUTypes.h"

/// Returns an NSString copying the characters from |bytes|, a C array of UTF8-encoded bytes.
/// Returns nil if |bytes| is NULL.
static NSString *GADUStringFromUTF8String(const char *bytes) { return bytes ? @(bytes) : nil; }

/// Returns a C string from a C array of UTF8-encoded bytes.
static const char *cStringCopy(const char *string) {
  if (!string) {
    return NULL;
  }
  char *res = (char *)malloc(strlen(string) + 1);
  strcpy(res, string);
  return res;
}

/// Returns a C string from a C array of UTF8-encoded bytes.
static const char **cStringArrayCopy(NSArray *array) {
  if (array == nil) {
    return nil;
  }

  const char **stringArray;

  stringArray = calloc(array.count, sizeof(char *));
  for (int i = 0; i < array.count; i++) {
    stringArray[i] = cStringCopy([array[i] UTF8String]);
  }
  return stringArray;
}

void GADUDisableMediationInitialization() {
  [[GADMobileAds sharedInstance] disableMediationInitialization];
}

/// Create an empty CreateRequestConfiguration
GADUTypeRequestConfigurationRef GADUCreateRequestConfiguration() {
    GADURequestConfiguration *requestConfiguration = [[GADURequestConfiguration alloc] init];
    GADUObjectCache *cache = [GADUObjectCache sharedInstance];
    cache[requestConfiguration.gadu_referenceKey] = requestConfiguration;
    return (__bridge GADUTypeRequestConfigurationRef)(requestConfiguration);
}

/// Set MobileAds RequestConfiguration
void GADUSetRequestConfiguration(GADUTypeRequestConfigurationRef requestConfiguration) {
    GADURequestConfiguration *internalRequestConfiguration =
        (__bridge GADURequestConfiguration *)requestConfiguration;
    GADMobileAds.sharedInstance.requestConfiguration.testDeviceIdentifiers =
        internalRequestConfiguration.testDeviceIdentifiers;
}

/// Set RequestConfiguration Test Device Ids
void GADUSetRequestConfigurationTestDeviceIdentifiers(
    GADUTypeRequestConfigurationRef requestConfiguration, const char **testDeviceIDs,
    NSInteger testDeviceIDLength) {
  GADURequestConfiguration *internalRequestConfiguration =
      (__bridge GADURequestConfiguration *)requestConfiguration;
  NSMutableArray *testDeviceIDsArray = [[NSMutableArray alloc] init];
  for (int i = 0; i < testDeviceIDLength; i++) {
    [testDeviceIDsArray addObject:GADUStringFromUTF8String(testDeviceIDs[i])];
  }
  [internalRequestConfiguration setTestDeviceIdentifiers:testDeviceIDsArray];
}

/// Returns List RequestConfiguration Test Device Ids
const char **GADUGetTestDeviceIdentifiers(GADUTypeRequestConfigurationRef requestConfiguration) {
  GADURequestConfiguration *internalRequestConfiguration =
      (__bridge GADURequestConfiguration *)requestConfiguration;
  NSArray<NSString *> *testDeviceIDs = internalRequestConfiguration.testDeviceIdentifiers;
  return cStringArrayCopy(testDeviceIDs);
}

/// Returns count of RequestConfiguration Test Device Ids
int GADUGetTestDeviceIdentifiersCount(GADUTypeRequestConfigurationRef requestConfiguration) {
  GADURequestConfiguration *internalRequestConfiguration =
      (__bridge GADURequestConfiguration *)requestConfiguration;
  NSArray<NSString *> *testDeviceIDs = internalRequestConfiguration.testDeviceIdentifiers;
  return testDeviceIDs.count;
}

/// Shows ad inspector UI.
void GADUPresentAdInspector(GADUTypeMobileAdsClientRef *mobileAdsClientRef,
                            GADUAdInspectorCompleteCallback adInspectorCompletionCallback) {
  UIViewController *unityController = [GADUPluginUtil unityGLViewController];
  [GADMobileAds.sharedInstance
      presentAdInspectorFromViewController:unityController
                         completionHandler:^(NSError *_Nullable error) {
                           if (adInspectorCompletionCallback) {
                             adInspectorCompletionCallback(mobileAdsClientRef,
                                                           (__bridge GADUTypeErrorRef)error);
                           }
                         }];
}

// AdError Methods

const int GADUGetAdErrorCode(GADUTypeErrorRef error) {
  NSError *internalError = (__bridge NSError *)error;
  return internalError.code;
}

const char *GADUGetAdErrorDomain(GADUTypeErrorRef error) {
  NSError *internalError = (__bridge NSError *)error;
  return cStringCopy(internalError.domain.UTF8String);
}

const char *GADUGetAdErrorMessage(GADUTypeErrorRef error) {
  NSError *internalError = (__bridge NSError *)error;
  return cStringCopy(internalError.localizedDescription.UTF8String);
}

const GADUTypeErrorRef GADUGetAdErrorUnderLyingError(GADUTypeErrorRef error) {
  NSError *internalError = (__bridge NSError *)error;
  NSError *underlyingError = internalError.userInfo[NSUnderlyingErrorKey];
  return (__bridge GADUTypeErrorRef)(underlyingError);
}

const char *GADUGetAdErrorDescription(GADUTypeErrorRef error) {
  NSError *internalError = (__bridge NSError *)error;
  return cStringCopy(internalError.description.UTF8String);
}

#pragma mark - Other methods
/// Removes an object from the cache.
void GADURelease(GADUTypeRef ref) {
  if (ref) {
    GADUObjectCache *cache = [GADUObjectCache sharedInstance];
    [cache removeObjectForKey:[(__bridge NSObject *)ref gadu_referenceKey]];
  }
}
