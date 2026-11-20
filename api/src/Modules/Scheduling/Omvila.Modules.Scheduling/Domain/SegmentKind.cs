namespace Omvila.Modules.Scheduling.Domain;

/// <summary>Meşguliyet diliminin türü (alan-modeli.md § AppointmentItemSegment.Kind).</summary>
internal enum SegmentKind
{
    BufferBefore,
    Stage,
    BufferAfter,
}
