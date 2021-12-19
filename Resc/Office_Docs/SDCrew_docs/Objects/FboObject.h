//
//  FboObject.h
//  SDCrew
//
//  Created by Arifur Rahman on 24/12/19.
//  Copyright © 2021 Satcom Direct, Inc. All rights reserved.
//

#import "YapDatabaseObject.h"

NS_ASSUME_NONNULL_BEGIN

@interface FboObject : YapDatabaseObject

@property (nonatomic) BOOL isUpdated;

@property (nonatomic, strong, nullable) NSString *localPhone;
@property (nonatomic, strong, nullable) NSString *fboId;
@property (nonatomic, strong, nullable) NSString *serviceEmailAddress;
@property (nonatomic, strong, nullable) NSString *type;
@property (nonatomic, strong, nullable) NSString *fboHandlerName;
@property (nonatomic, strong, nullable) NSString *icao;
@property (nonatomic, strong, nullable) NSString *icaoId;
- (void)didChangeValueForKey:(NSString *)key;
+ (BOOL)automaticallyNotifiesObserversForKey:(NSString *)key;
-(FboObject *)duplicate;

@end

NS_ASSUME_NONNULL_END
