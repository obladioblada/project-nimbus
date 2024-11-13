namespace MetadataHandlerLambda.Interfaces;

public interface IMetadataService
{
    
    /// <summary>
    /// Store the metadata
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task Save(CancellationToken cancellationToken);

    /// <summary>
    /// Delete the metadata
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task Delete(CancellationToken cancellationToken);
}