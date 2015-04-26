package tv.awesomepeople.virtualmuseum;

import com.unity3d.player.UnityPlayerActivity;

import android.util.Log;
import android.content.Intent;
import android.os.Bundle;

public class FileBrowser extends UnityPlayerActivity
{
	private String path = "path";

	@Override
	public void onCreate(Bundle savedInstanceState)
	{
        super.onCreate(savedInstanceState);
		Log.i("Unity", "onCreate called");
	}

	public void startBrowser() {
    	Intent intent = new Intent(Intent.ACTION_GET_CONTENT);
    	intent.setType("*/*");
    	startActivityForResult(intent, 0);
        Log.i("Unity", "intent send = " + path);
	}

	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
    	super.onActivityResult(requestCode, resultCode, data);
     	path = data.getDataString();
		Log.i("Unity", "onActivityResult returns = " + path);
	}

    public String getPath() {
        return path;
    }
}