namespace MetadataHandlerLambda.Interfaces;

public interface IMetadataService
{
    /// <summary>
    /// Store the metadata
    /// </summary>
    /// <param name="fileMetadata">The metadata to store</param>
    /// <returns></returns>
    Task<FileMetadata> Save(FileMetadata fileMetadata);
}