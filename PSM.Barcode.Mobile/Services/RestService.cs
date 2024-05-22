namespace PSM.Barcode.Services;

public class RestService
{
	private readonly OptionsService _options;

	public RestService(OptionsService options)
    {
		_options = options;
	}
}