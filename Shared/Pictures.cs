namespace TinderButForBartering;

public class Pictures
{
    public static async Task<MemoryStream> GetPhoto(bool withCamera)
    {
        // handle denial of request and errors!
        FileResult photo;
        if (withCamera)
        {
            await Permissions.RequestAsync<Permissions.Camera>();
            await Permissions.RequestAsync<Permissions.StorageWrite>();
            await Permissions.RequestAsync<Permissions.StorageRead>();
            photo = await MediaPicker.Default.CapturePhotoAsync();
        }
        else
        {
            await Permissions.RequestAsync<Permissions.Media>(); // necessary?
            await Permissions.RequestAsync<Permissions.StorageWrite>();
            await Permissions.RequestAsync<Permissions.StorageRead>();
            photo = await MediaPicker.Default.PickPhotoAsync();
        }        
        
        if (photo != null)
        {
            Stream sourceStream = await photo.OpenReadAsync();
            MemoryStream imageStream = new MemoryStream();
            await sourceStream.CopyToAsync(imageStream)
                .ContinueWith(task => { sourceStream.Dispose(); });
            imageStream.Position = 0;
            return imageStream;
        }
        return null;
    }
}