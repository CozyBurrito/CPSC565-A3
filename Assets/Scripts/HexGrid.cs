using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{

    static public Color[] colors = { Color.white, Color.blue, Color.green, Color.yellow };
    public int width = 6;
    public int height = 6;

    public Color defaultColor = Color.white;

    public HexCell cellPrefab;
    public Text cellLabelPrefab;

    HexCell[] cells;

    Canvas gridCanvas;
    HexMesh hexMesh;

	public int id;
	public bool isSet = false;

    public string gridColouringStr = ""; // A representation of the colours of each cell in this grid. Also called the genome string

    public void ColorCell(Vector3 position, Color color)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        HexCell cell = cells[index];
        cell.color = color;
        hexMesh.Triangulate(cells);
    }

    void CreateCell(int x, int z, int i, Color c)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = c;

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();

    }

    // This function is used to create a HexGrid from the predefined gridColouringStr
    public void createHexGridFromString(int gridId)
    {
		if(gridColouringStr.Length !=  height * width)
		{
			Debug.LogError("gridColouringStr not set correctly for this HexGrid!");
			return;
		}

        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();

        cells = new HexCell[height * width];
		this.id = gridId;

        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Color c = colors[0];
                if (gridColouringStr[i] == 'W')
                    c = colors[0];
                else if (gridColouringStr[i] == 'B')
                    c = colors[1];
                else if (gridColouringStr[i] == 'G')
                    c = colors[2];
                else if (gridColouringStr[i] == 'Y')
                    c = colors[3];

                CreateCell(x, z, i++, c);
            }
        }

        hexMesh.Triangulate(cells);
		hexMesh.disableMesh();

		gridCanvas.gameObject.SetActive(false);

		BoxCollider collider = GetComponent<BoxCollider>();
		collider.center = new Vector3(47.6f, 0f, 37.8f);
		collider.size = new Vector3(112.7f, 0f, 95.6f);
    }

	// This function is used to create a random colouring for each cell in the HexGrid
    public void randomizeGridColouring()
    {
        string gridColours = "";

        for (int i = 0; i < (width * height); i++)
        {
            Color c = colors[(int)Random.Range(0, colors.Length)];

            if (c.Equals(Color.white))
                gridColours += "W";
            else if (c.Equals(Color.blue))
                gridColours += "B";
            else if (c.Equals(Color.green))
                gridColours += "G";
            else if (c.Equals(Color.yellow))
                gridColours += "Y";
        }

		gridColouringStr = gridColours;
    }

	public void highlight()
	{
		float x = this.transform.position.x;
		float y = 4f;
		float z = this.transform.position.z;
		this.gameObject.transform.SetPositionAndRotation(new Vector3(x, y, z), Quaternion.identity);
		
	}

	public void unHighlight()
	{
		float x = this.transform.position.x;
		float y = 0f;
		float z = this.transform.position.z;
		this.gameObject.transform.SetPositionAndRotation(new Vector3(x, y, z), Quaternion.identity);
	}

	private void OnMouseOver() {
		highlight();
	}

	private void OnMouseExit() {
		if(!isSet)
			unHighlight();
	}

	private void OnMouseDown() {
		isSet = this.gameObject.GetComponentInParent<EvolutionManager>().setSelectedHexGrid(this.id);
	}

}