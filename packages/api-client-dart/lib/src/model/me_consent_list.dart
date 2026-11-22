//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/me_consent.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'me_consent_list.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeConsentList {
  /// Returns a new [MeConsentList] instance.
  MeConsentList({

    required  this.items,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<MeConsent> items;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeConsentList &&
      other.items == items;

    @override
    int get hashCode =>
        items.hashCode;

  factory MeConsentList.fromJson(Map<String, dynamic> json) => _$MeConsentListFromJson(json);

  Map<String, dynamic> toJson() => _$MeConsentListToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

