namespace TinderButForBartering;

public class Pictures
{
    public static async Task TakePhoto(Image imageField)
    {
        // handle denial of request and errors!
        await Permissions.RequestAsync<Permissions.Camera>();
        await Permissions.RequestAsync<Permissions.StorageWrite>();
        await Permissions.RequestAsync<Permissions.StorageRead>();
        FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
        if (photo != null)
        {
            Stream sourceStream = await photo.OpenReadAsync(); // a problem was solved by removing "using" from the beginning of this line. Might that cause other problems?
            imageField.Source = ImageSource.FromStream(() => sourceStream);
        }
    }

    public static async Task SelectPhoto(Image imageField)
    {
        // handle denial of request and errors!
        await Permissions.RequestAsync<Permissions.Media>(); //
        await Permissions.RequestAsync<Permissions.StorageWrite>();
        await Permissions.RequestAsync<Permissions.StorageRead>();
        FileResult photo = await MediaPicker.Default.PickPhotoAsync();
        if (photo != null)
        {
            Stream sourceStream = await photo.OpenReadAsync(); // a problem was solved by removing "using" from the beginning of this line. Might that cause other problems?
            imageField.Source = ImageSource.FromStream(() => sourceStream);
        }
    }
}

