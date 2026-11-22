//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/audit_log_entry_actor.dart';
import 'package:omvila_api/src/model/admin_brand_ref.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'audit_log_entry.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AuditLogEntry {
  /// Returns a new [AuditLogEntry] instance.
  AuditLogEntry({

    required  this.id,

    required  this.occurredUtc,

    required  this.actor,

    required  this.action,

    required  this.entityType,

    required  this.entityId,

    required  this.brand,

    required  this.changes,
  });

      /// Kayıt numarası
  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final int id;



      /// \"Zaman\"
  @JsonKey(
    
    name: r'occurredUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime occurredUtc;



  @JsonKey(
    
    name: r'actor',
    required: true,
    includeIfNull: false,
  )


  final AuditLogEntryActor actor;



      /// Eylem
  @JsonKey(
    
    name: r'action',
    required: true,
    includeIfNull: false,
  unknownEnumValue: AuditLogEntryActionEnum.unknownDefaultOpenApi,
  )


  final AuditLogEntryActionEnum action;



      /// Varlık türü (Brand, PaymentNotice, CreditTransaction, Subscription, BrandSmsSender, BrandCustomer…)
  @JsonKey(
    
    name: r'entityType',
    required: true,
    includeIfNull: false,
  )


  final String entityType;



      /// Varlık kimliği
  @JsonKey(
    
    name: r'entityId',
    required: true,
    includeIfNull: true,
  )


  final String? entityId;



      /// \"Marka\"; marka dışı kayıtta null
  @JsonKey(
    
    name: r'brand',
    required: true,
    includeIfNull: true,
  )


  final AdminBrandRef? brand;



      /// \"Ayrıntı\": değişen alanlar ve bağlam (telefon ve ad maskeli)
  @JsonKey(
    
    name: r'changes',
    required: true,
    includeIfNull: false,
  )


  final Map<String, Object> changes;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AuditLogEntry &&
      other.id == id &&
      other.occurredUtc == occurredUtc &&
      other.actor == actor &&
      other.action == action &&
      other.entityType == entityType &&
      other.entityId == entityId &&
      other.brand == brand &&
      other.changes == changes;

    @override
    int get hashCode =>
        id.hashCode +
        occurredUtc.hashCode +
        actor.hashCode +
        action.hashCode +
        entityType.hashCode +
        (entityId == null ? 0 : entityId.hashCode) +
        (brand == null ? 0 : brand.hashCode) +
        changes.hashCode;

  factory AuditLogEntry.fromJson(Map<String, dynamic> json) => _$AuditLogEntryFromJson(json);

  Map<String, dynamic> toJson() => _$AuditLogEntryToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Eylem
enum AuditLogEntryActionEnum {
@JsonValue(r'Create')
create(r'Create'),
@JsonValue(r'Update')
update(r'Update'),
@JsonValue(r'Delete')
delete(r'Delete'),
@JsonValue(r'View')
view(r'View'),
@JsonValue(r'Approve')
approve(r'Approve'),
@JsonValue(r'Match')
match(r'Match'),
@JsonValue(r'Export')
export_(r'Export'),
@JsonValue(r'Anonymize')
anonymize(r'Anonymize'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const AuditLogEntryActionEnum(this.value);

final String value;

@override
String toString() => value;
}


