//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/walk_in_ticket_view.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'walk_in_queue.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class WalkInQueue {
  /// Returns a new [WalkInQueue] instance.
  WalkInQueue({

    required  this.branchId,

    required  this.date,

    required  this.timeZone,

    required  this.generatedUtc,

    required  this.items,

    required  this.waitingCount,

     this.oldestJoinedUtc,

     this.estimatedWaitMinutesForNew,

    required  this.servedCount,

    required  this.nextQueueNumber,
  });

  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



  @JsonKey(
    
    name: r'date',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime date;



      /// IANA saat dilimi
  @JsonKey(
    
    name: r'timeZone',
    required: true,
    includeIfNull: false,
  )


  final String timeZone;



      /// Sunucunun \"şimdi\"si; bekleme süreleri buna göre sayılır
  @JsonKey(
    
    name: r'generatedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime generatedUtc;



      /// Sırada bekleyenler (Waiting, Called), numara sırasıyla
  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<WalkInTicketView> items;



      /// \"Sırada\"
  @JsonKey(
    
    name: r'waitingCount',
    required: true,
    includeIfNull: false,
  )


  final int waitingCount;



      /// \"en eskisi 12 dk\"
  @JsonKey(
    
    name: r'oldestJoinedUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? oldestJoinedUtc;



      /// \"Yeni gelen için ~35 dk\"; müsait personel yoksa null
  @JsonKey(
    
    name: r'estimatedWaitMinutesForNew',
    required: false,
    includeIfNull: false,
  )


  final int? estimatedWaitMinutesForNew;



      /// \"Bugün sıradan · randevuya çevrildi\"
  @JsonKey(
    
    name: r'servedCount',
    required: true,
    includeIfNull: false,
  )


  final int servedCount;



      /// Sıraya ekle ekranındaki \"No 11\" (gösterim; numara oluşturmada atomik verilir)
  @JsonKey(
    
    name: r'nextQueueNumber',
    required: true,
    includeIfNull: false,
  )


  final int nextQueueNumber;





    @override
    bool operator ==(Object other) => identical(this, other) || other is WalkInQueue &&
      other.branchId == branchId &&
      other.date == date &&
      other.timeZone == timeZone &&
      other.generatedUtc == generatedUtc &&
      other.items == items &&
      other.waitingCount == waitingCount &&
      other.oldestJoinedUtc == oldestJoinedUtc &&
      other.estimatedWaitMinutesForNew == estimatedWaitMinutesForNew &&
      other.servedCount == servedCount &&
      other.nextQueueNumber == nextQueueNumber;

    @override
    int get hashCode =>
        branchId.hashCode +
        date.hashCode +
        timeZone.hashCode +
        generatedUtc.hashCode +
        items.hashCode +
        waitingCount.hashCode +
        (oldestJoinedUtc == null ? 0 : oldestJoinedUtc.hashCode) +
        (estimatedWaitMinutesForNew == null ? 0 : estimatedWaitMinutesForNew.hashCode) +
        servedCount.hashCode +
        nextQueueNumber.hashCode;

  factory WalkInQueue.fromJson(Map<String, dynamic> json) => _$WalkInQueueFromJson(json);

  Map<String, dynamic> toJson() => _$WalkInQueueToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

