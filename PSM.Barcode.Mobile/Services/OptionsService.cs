using PSM.Barcode.DB;
using PSM.Barcode.Models;

namespace PSM.Barcode.Services;

public class OptionsService(IPreferences preferences, DbCtx ctx)
{
	private readonly IPreferences _preferences = preferences;
	private readonly DbCtx _ctx = ctx;

	public string ServerHost
	{
		get => _preferences.Get(nameof(ServerHost), "");
		private set => _preferences.Set(nameof(ServerHost), value);
	}
	public string ServerIP
	{
		get => _preferences.Get(nameof(ServerIP), "");
		private set => _preferences.Set(nameof(ServerIP), value);
	}
	public bool UseHost
	{
		get => _preferences.Get(nameof(UseHost), true);
		private set => _preferences.Set(nameof(UseHost), value);
	}
	public int SavedUserId
	{
		get => _preferences.Get(nameof(SavedUserId), 0);
		private set => _preferences.Set(nameof(SavedUserId), value);
	}


	public void SetServer(string server)
	{
		var pair = server.Split('|');
		ServerHost = pair[0];
		ServerIP = pair[1];

		ServerChanged?.Invoke(this, new());
	}
	public void SetUserId(int userId)
	{
		SavedUserId = userId;
		_savedUser = null;
		UserChanged?.Invoke(this, new());
	}

	public string BaseUri => UseHost ? ServerHost : ServerIP;

	private User? _savedUser = null;
	public User? SavedUser => _savedUser ??= _ctx.Users.FirstOrDefault(u => u.UserId == SavedUserId);

	public delegate void ServerChangedEventHandler(object sender, EventArgs e);
	public delegate void UserChangedEventHandler(object sender, EventArgs e);

	public event ServerChangedEventHandler? ServerChanged;
	public event UserChangedEventHandler? UserChanged;
}
