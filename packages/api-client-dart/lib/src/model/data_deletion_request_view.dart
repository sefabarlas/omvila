//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'data_deletion_request_view.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class DataDeletionRequestView {
  /// Returns a new [DataDeletionRequestView] instance.
  DataDeletionRequestView({

    required  this.id,

     this.brandCustomerId,

    required  this.customerDisplayName,

    required  this.requestedVia,

    required  this.requestedUtc,

    required  this.dueOn,

    required  this.status,

     this.completedUtc,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



      /// Tamamlanan talepte null
  @JsonKey(
    
    name: r'brandCustomerId',
    required: false,
    includeIfNull: false,
  )


  final String? brandCustomerId;



      /// Tamamlanan talepte maskeli (\"A•••• K••••\")
  @JsonKey(
    
    name: r'customerDisplayName',
    required: true,
    includeIfNull: false,
  )


  final String customerDisplayName;



      /// \"Uygulamadan hesap silme\" / \"Panelden anonimleştirme\"
  @JsonKey(
    
    name: r'requestedVia',
    required: true,
    includeIfNull: false,
  unknownEnumValue: DataDeletionRequestViewRequestedViaEnum.unknownDefaultOpenApi,
  )


  final DataDeletionRequestViewRequestedViaEnum requestedVia;



  @JsonKey(
    
    name: r'requestedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime requestedUtc;



      /// Son gün (DeletionRequestDueDays); \"Bekliyor · 28 gün\"
  @JsonKey(
    
    name: r'dueOn',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime dueOn;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: DataDeletionRequestViewStatusEnum.unknownDefaultOpenApi,
  )


  final DataDeletionRequestViewStatusEnum status;



  @JsonKey(
    
    name: r'completedUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? completedUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is DataDeletionRequestView &&
      other.id == id &&
      other.brandCustomerId == brandCustomerId &&
      other.customerDisplayName == customerDisplayName &&
      other.requestedVia == requestedVia &&
      other.requestedUtc == requestedUtc &&
      other.dueOn == dueOn &&
      other.status == status &&
      other.completedUtc == completedUtc;

    @override
    int get hashCode =>
        id.hashCode +
        (brandCustomerId == null ? 0 : brandCustomerId.hashCode) +
        customerDisplayName.hashCode +
        requestedVia.hashCode +
        requestedUtc.hashCode +
        dueOn.hashCode +
        status.hashCode +
        (completedUtc == null ? 0 : completedUtc.hashCode);

  factory DataDeletionRequestView.fromJson(Map<String, dynamic> json) => _$DataDeletionRequestViewFromJson(json);

  Map<String, dynamic> toJson() => _$DataDeletionRequestViewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// \"Uygulamadan hesap silme\" / \"Panelden anonimleştirme\"
enum DataDeletionRequestViewRequestedViaEnum {
@JsonValue(r'CustomerApp')
customerApp(r'CustomerApp'),
@JsonValue(r'Panel')
panel(r'Panel'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const DataDeletionRequestViewRequestedViaEnum(this.value);

final String value;

@override
String toString() => value;
}


enum DataDeletionRequestViewStatusEnum {
@JsonValue(r'Pending')
pending(r'Pending'),
@JsonValue(r'Completed')
completed(r'Completed'),
@JsonValue(r'Rejected')
rejected(r'Rejected'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const DataDeletionRequestViewStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


