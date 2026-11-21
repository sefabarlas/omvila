//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/manage_session_brand.dart';
import 'package:omvila_api/src/model/manage_session_membership.dart';
import 'package:omvila_api/src/model/subscription_banner.dart';
import 'package:omvila_api/src/model/manage_session_user.dart';
import 'package:omvila_api/src/model/branch_ref.dart';
import 'package:omvila_api/src/model/manage_session_sms_credits.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_session.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageSession {
  /// Returns a new [ManageSession] instance.
  ManageSession({

    required  this.user,

    required  this.brand,

    required  this.membership,

    required  this.permissions,

    required  this.branches,

    required  this.subscription,

     this.smsCredits,
  });

  @JsonKey(
    
    name: r'user',
    required: true,
    includeIfNull: false,
  )


  final ManageSessionUser user;



  @JsonKey(
    
    name: r'brand',
    required: true,
    includeIfNull: false,
  )


  final ManageSessionBrand brand;



  @JsonKey(
    
    name: r'membership',
    required: true,
    includeIfNull: false,
  )


  final ManageSessionMembership membership;



  @JsonKey(
    
    name: r'permissions',
    required: true,
    includeIfNull: false,
  )


  final List<String> permissions;



  @JsonKey(
    
    name: r'branches',
    required: true,
    includeIfNull: false,
  )


  final List<BranchRef> branches;



  @JsonKey(
    
    name: r'subscription',
    required: true,
    includeIfNull: false,
  )


  final SubscriptionBanner subscription;



  @JsonKey(
    
    name: r'smsCredits',
    required: false,
    includeIfNull: false,
  )


  final ManageSessionSmsCredits? smsCredits;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageSession &&
      other.user == user &&
      other.brand == brand &&
      other.membership == membership &&
      other.permissions == permissions &&
      other.branches == branches &&
      other.subscription == subscription &&
      other.smsCredits == smsCredits;

    @override
    int get hashCode =>
        user.hashCode +
        brand.hashCode +
        membership.hashCode +
        permissions.hashCode +
        branches.hashCode +
        subscription.hashCode +
        smsCredits.hashCode;

  factory ManageSession.fromJson(Map<String, dynamic> json) => _$ManageSessionFromJson(json);

  Map<String, dynamic> toJson() => _$ManageSessionToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

