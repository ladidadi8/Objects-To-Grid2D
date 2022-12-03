using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class WaypointGenerator : MonoBehaviour
{
    [Header("Waypoint Actions")]
    //triggers
    [SerializeField] private bool generatePositionsArray = false;
    [SerializeField] private bool randomizePositions = false;
    [SerializeField] private bool resetPositions = false;

    [Header("Grid Settings")]
    [SerializeField] private Transform refWaypoint;
    [SerializeField] private int waypointsPerRow = 1; //minimum amount is 1.
    [SerializeField] private float rowOffsetY; //distance between rows
    [SerializeField] private float randomAmount = .25f;  // randomize amount

    [SerializeField] private Transform[] waypoints; //array of waypoint objects hierarchy, fill in inspector

    [Header("Hard Reset")]
    [SerializeField] private bool forceReset = false;
    [SerializeField] private bool keepWaypoints = false;
    [SerializeField] private bool areYouSure = false;

    [Header("Debug Data")]
    [SerializeField] [ReadOnly] private bool positionsExist = false; //true if positions are generated
    [SerializeField] [ReadOnly] private bool waypointsPositioned = false; //true if waypoints are positioned on grid
    [SerializeField] [ReadOnly] private int positionsCalculated = 0;

    private Vector2[] waypointPositions; //array of desired waypoint positions

    private void Update()
    {
        //use only if some error occurs
        if (forceReset && areYouSure) ResetEverything();

        //double check if conditions make sense. Error can cause exit from function before correct condition is set.
        if (resetPositions && waypoints == null) resetPositions = false; //nothing to clear if array is empty
        if (waypointPositions == null && positionsExist) positionsExist = false; //if positions array is null grid can't exist
        if (randomizePositions && !waypointsPositioned) randomizePositions = false; //reset bool if positions arent generated

        if (generatePositionsArray) GeneratePositions(); //Generate Vector2 positions and add them to positions array
        else if (resetPositions && waypoints != null) ClearPositions(); //sets waypoint position to refWaypoint and resets positions array

        if (positionsExist)
        {
            if (randomizePositions && waypointsPositioned) RandomizePositions(); //randomize waypoint position around its position in position array
        }
    }

    private void GeneratePositions()
    {
        if (waypointsPerRow <= 0)
        {
            Debug.LogWarning("cant have less than 1 point per row");
            return;
        }
        else if (waypointsPerRow > waypoints.Length)
        {
            Debug.LogWarning("cant have more points per row than actual points in array, amount: " + waypointsPerRow +
                             "\n changing amount of points to waypoint.length: " + waypoints.Length);

            waypointsPerRow = waypoints.Length;
        }

        waypointPositions = new Vector2[waypoints.Length];

        float hLength = Mathf.Abs(refWaypoint.position.x) * 2f; // length from left to right side of screen in world units
        float spacingDistance = (hLength) / (waypointsPerRow + 1);  //distance between two waypoints on x axis
        float spacingVariation = spacingDistance / 2f; // variation each row. by default each 2nd row is offset on x axis by half of spacing distance

        ///   .   .   .   .
        /// .   .   .   .
        ///   .   .   .   .      -> variation added to spacing each 2nd row
        /// .   .   .   .        -> default spacing

        int columnIndex = 0; // index of each waypoint column
        bool variationAdded = false; // true every 2nd row.

        // starting x position of refWaypoint, modified each loop by spacingDistance. 
        // every fresh row reset it to starting value which is stored in -> xPosReference
        float xPos = refWaypoint.position.x - spacingVariation / 2f; 
        float yPos = refWaypoint.position.y; // y position of refWaypoint, modified every new row by rowOffsetY

        float xPosReference = xPos; // reference to starting x position, used each fresh row

        for (int i = 0; i < waypoints.Length; i++)
        {
            //if points per row is equals to current amount of points in this row then offset it on Y and reset X coordinate to starting position
            if (columnIndex == waypointsPerRow)
            {
                columnIndex = 0;

                //if fresh row is added and we are not using variation on x, then use it for this row
                if (!variationAdded)
                {
                    xPos = xPosReference + spacingVariation;
                    variationAdded = true;
                }
                else
                {
                    variationAdded = false;
                    xPos = xPosReference;
                }

                //add offset for new row
                yPos += rowOffsetY;
            }

            //adding to previous xPos, xPos resets on new row
            xPos += spacingDistance;

            Vector2 finalPosition = new Vector2(xPos, yPos); 

            //adding position to array
            waypointPositions[i] = finalPosition;

            columnIndex++; 
        }
        positionsCalculated = waypointPositions.Length;
        generatePositionsArray = false;
        positionsExist = true;

        SetWaypointsToPositions();
    }

    public void SetWaypointsToPositions()
    {
        for(int i = 0; i < waypoints.Length; i++)
        {
            if(waypoints[i] != null)
            {
                waypoints[i].position = waypointPositions[i];
            }
        }
        waypointsPositioned = true;
    }

    private void RandomizePositions()
    {
        for(int i = 0; i < waypoints.Length; i++)
        {
            float xRandom = waypointPositions[i].x + Random.Range(-randomAmount, randomAmount);
            float yRandom = waypointPositions[i].y + Random.Range(-randomAmount, randomAmount);
            waypoints[i].position = new Vector2(xRandom, yRandom);
        }
        randomizePositions = false;
    }

    private void ClearPositions()
    {
        waypointPositions = new Vector2[0];

        for(int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i].position = refWaypoint.position;
        }

        positionsCalculated = 0;
        waypointsPositioned = false;
        resetPositions = false;
        positionsExist = false;
    }

    private void ResetEverything()
    {
        if (keepWaypoints)
        {
            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i].position = refWaypoint.position;
            }
        }
        else waypoints = new Transform[0];

        waypointPositions = new Vector2[0];
        randomizePositions = false;
        generatePositionsArray = false;
        resetPositions = false;
        waypointsPositioned = false;
        positionsExist = false;
        positionsCalculated = 0;

        forceReset = false;
        areYouSure = false;
        keepWaypoints = false;
    }


}

#region GreyedOutPublicFields
public class ReadOnlyAttribute : PropertyAttribute
{

}

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
    public override void OnGUI(Rect position,
                               SerializedProperty property,
                               GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endregion