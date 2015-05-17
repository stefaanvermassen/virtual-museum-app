package tv.awesomepeople.museumplugin;

import com.unity3d.player.UnityPlayerActivity;
import android.content.Intent;
import android.net.Uri;
import android.app.Activity;

public class MuseumMainActivity extends UnityPlayerActivity {

    private String path = null;

    public String getIntentDataString() {
        return getIntent().getDataString();
    }

    public void startBrowser() {
        Intent intent = new Intent(Intent.ACTION_GET_CONTENT);
        Uri uri = Uri.parse(android.os.Environment.DIRECTORY_PICTURES);
        intent.setDataAndType(uri, "image/*");
        startActivityForResult(intent, 0);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        
        if (data != null) {
            path = data.getDataString();
        } else {
            path = "";
        }
    }

    public String getPath() {
        return path;
    }

    public void setPath(String path) {
        this.path = path;
    }
}