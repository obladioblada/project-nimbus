namespace MetadataHandlerLambda.Interfaces;

public interface IMetadataService
{
    /// <summary>
    /// Store the metadata
    /// </summary>
    /// <param name="fileMetadata">The metadata to store</param>
    /// <returns></returns>
    Task Save(FileMetadata fileMetadata);

    /// <summary>
    /// Delete the metadata
    /// </summary>
    /// <param name="file">The name of the file to delete</param>
    /// <returns></returns>
    Task Delete(string file);
}