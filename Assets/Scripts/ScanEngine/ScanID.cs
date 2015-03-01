
/**
 * Stub for testing, just returns the filter string;
 * */
class ScanID : ScanIdentity
{
    private string s;

	public ScanID (string s):base(ScanTechnology.ID)
	{
        this.s = s;
	}

    public string getString(){
        return s;
    }
}

