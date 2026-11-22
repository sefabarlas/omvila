//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'cash_day_close_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CashDayCloseRequest {
  /// Returns a new [CashDayCloseRequest] instance.
  CashDayCloseRequest({

    required  this.branchId,

    required  this.date,

    required  this.countedCash,

     this.differenceNote,
  });

  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



      /// Kapatılan yerel gün; bugünden sonra olamaz
  @JsonKey(
    
    name: r'date',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime date;



      /// \"Sayılan nakit\"
  @JsonKey(
    
    name: r'countedCash',
    required: true,
    includeIfNull: false,
  )


  final Money countedCash;



      /// \"Fark açıklaması\"; sayılan ile beklenen farklıysa zorunlu
  @JsonKey(
    
    name: r'differenceNote',
    required: false,
    includeIfNull: true,
  )


  final String? differenceNote;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CashDayCloseRequest &&
      other.branchId == branchId &&
      other.date == date &&
      other.countedCash == countedCash &&
      other.differenceNote == differenceNote;

    @override
    int get hashCode =>
        branchId.hashCode +
        date.hashCode +
        countedCash.hashCode +
        (differenceNote == null ? 0 : differenceNote.hashCode);

  factory CashDayCloseRequest.fromJson(Map<String, dynamic> json) => _$CashDayCloseRequestFromJson(json);

  Map<String, dynamic> toJson() => _$CashDayCloseRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

