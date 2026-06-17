using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

internal sealed class MegaAttributes
{
    [JsonPropertyName(name: "n")] public string? Name { get; set; }

    [JsonPropertyName(name: "c")] public string? SerializedFingerprint { get; set; }

    [JsonIgnore] public DateTime? ModificationDate { get; private set; }

    public static MegaAttributes Create(string? name, Stream stream, DateTime? modificationDate)
    {
        var attrs = new MegaAttributes
        {
            Name = name
        };

        if (modificationDate is null)
        {
            return attrs;
        }

        var fingerprint = new byte[25];
        Buffer.BlockCopy(MegaCrc32.ComputeMegaCrc(stream), srcOffset: 0, fingerprint, dstOffset: 0, count: 16);

        var epochBytes = modificationDate.Value.ToEpochSeconds().SerializeToBytes();
        Buffer.BlockCopy(epochBytes, srcOffset: 0, fingerprint, dstOffset: 16, epochBytes.Length);

        Array.Resize(ref fingerprint, fingerprint.Length - 9 + epochBytes.Length);
        attrs.SerializedFingerprint = fingerprint.ToBase64Url();
        attrs.ModificationDate = modificationDate.Value;

        return attrs;
    }

    public void HydrateAfterDeserialize()
    {
        if (SerializedFingerprint is null)
        {
            return;
        }

        var bytes = SerializedFingerprint.FromBase64Url();
        ModificationDate = bytes.DeserializeToLong(index: 16, bytes.Length - 16).FromEpochSeconds();
    }
}