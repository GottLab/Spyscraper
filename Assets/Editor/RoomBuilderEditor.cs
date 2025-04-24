#if UNITY_EDITOR
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ProBuilder;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class RoomBuilderMenu : EditorWindow
{

   

    float width = 10f;
    float height = 3f;
    float depth = 10f;
    float wallThickness = 0.1f;

    public Material wallMaterial;

    public Material floorMaterial;

    //LIGHT PROBES

    Vector3 probePerMeter = Vector3.one;

    //DOOR WALL SECTION
    
    float door_width = 10.0f;

    bool cut = false;
    
    ReorderableList doorInfoList; 

    List<Vector2> doorInfos  = new List<Vector2>();

    [MenuItem("Tools/ProBuilder/Room Builder")]
    public static void ShowWindow()
    {
        GetWindow<RoomBuilderMenu>("Room Builder");
    }

    void OnEnable()
    {
        doorInfoList = new ReorderableList(doorInfos, typeof(Vector2), true, true, true, true);
        doorInfoList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Doors");
        };

        doorInfoList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            doorInfos[index] = EditorGUI.Vector2Field(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                "", doorInfos[index]);
        };

        doorInfoList.onAddCallback = (ReorderableList list) =>
        {
            doorInfos.Add(Vector2.zero);
        };

        doorInfoList.onRemoveCallback = (ReorderableList list) =>
        {
            doorInfos.RemoveAt(list.index);
        };
    }

    void initMesh(ProBuilderMesh mesh, bool collider = true, Material material = null, GameObject parent = null)
    {

        if(material == null)
            material = BuiltinMaterials.defaultMaterial;
        if(parent == null)
            parent = Selection.activeGameObject;

        mesh.gameObject.isStatic = true;
        mesh.ToMesh();
        mesh.Refresh();
        mesh.Optimize();
        if(collider)
            mesh.gameObject.AddComponent<MeshCollider>();
        mesh.GetComponent<MeshRenderer>().sharedMaterial = material;

        if(parent != null)
        {
            mesh.transform.parent = parent.transform;
            mesh.transform.localPosition = Vector3.zero;
        }

    }


    void OnGUI()
    {
        GUILayout.Label("Room Parameters", EditorStyles.boldLabel);

        width = EditorGUILayout.FloatField("Width", width);
        height = EditorGUILayout.FloatField("Height", height);
        depth = EditorGUILayout.FloatField("Depth", depth);
        wallThickness = EditorGUILayout.FloatField("Wall Thickness", wallThickness);

        wallMaterial = (Material)EditorGUILayout.ObjectField(
            "Wall Material",
            wallMaterial,
            typeof(Material),
            false // don't allow scene objects
        );

        floorMaterial = (Material)EditorGUILayout.ObjectField(
            "Floor Material",
            floorMaterial,
            typeof(Material),
            false // don't allow scene objects
        );

        this.probePerMeter = EditorGUILayout.Vector3Field("Probes Per Meter", this.probePerMeter);

        
        GUILayout.Space(10);

        if (GUILayout.Button("Generate Room"))
        {
            GenerateRoom(width, height, depth, wallThickness);
        }

        if (doorInfoList != null)
        {
            doorInfoList.DoLayoutList();
        }

        door_width = EditorGUILayout.FloatField("Door Width", door_width);

        cut = EditorGUILayout.Toggle("Internal", cut);


        if (GUILayout.Button("Generate Door Wall"))
        {
            GenerateDoorWall(door_width, height, wallThickness, this.doorInfos);
        }
    }


    void GenerateDoorWall(float width, float height, float depth, List<Vector2> doorsInfo)
    {
        if(doorsInfo.Count == 0)
            return;

        //we remove the wall thickness since it needs to stay inside
        if(cut)
            width -= wallThickness * 2.0f;


        float doorTotalWidth = width / doorsInfo.Count;


        List<ProBuilderMesh> doors = new();

        for (int i = 0; i < doorsInfo.Count; i++)
        {
            Vector2 info = doorsInfo[i];
            ProBuilderMesh pbMesh = ShapeGenerator.GenerateDoor(PivotLocation.Center, doorTotalWidth, height, height - info.y, (doorTotalWidth - info.x) * 0.5f, depth);
            pbMesh.transform.position = new Vector3(doorTotalWidth * i,0,0);
            pbMesh.GetComponent<Renderer>().sharedMaterial = this.wallMaterial;
            doors.Add(pbMesh);

        }
        ProBuilderMesh m = doors.First();
        CombineMeshes.Combine(doors, m);
        VertexEditing.WeldVertices(m, m.faces.SelectMany(x => x.indexes).ToArray(), 0.1F);
        initMesh(m, true, wallMaterial);

        for (int i = 1; i < doorsInfo.Count; i++)
        {
            DestroyImmediate(doors[i].gameObject);
        }


        bool sameYAndX(Vector3 a, Vector3 b, int front)
        {
            bool correctHeight = Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.y, height * 0.5f);
            bool correctWidth = !cut && Mathf.Approximately(a.x, b.x);
            bool correctDepth = Mathf.Approximately(a.z, b.z) && Mathf.Approximately(a.z, wallThickness * 0.5f * front);
            return (correctWidth ^ correctHeight) && correctDepth;
        }

       m.faces.SelectMany(face => face.edges).Where(edge => 
            {
                var a = m.positions[edge.a];
                var b = m.positions[edge.b];
                return sameYAndX(a, b, 1);
            }
        ).ToList().ForEach(edge => {
            m.faces.SelectMany(face => face.edges).FirstOrDefault(e => 
            {
                var a = m.positions[e.a];
                var b = m.positions[e.b];
                var mainEdgeA = m.positions[edge.a];
                return sameYAndX(a,b, -1) && AppendElements.Bridge(m, edge, e, false) != null;
            });
        });
        m.ToMesh();
        m.Refresh();
        m.Optimize();
        
    }

    void GenerateRoom(float width, float height, float depth, float wallThickness)
    {
        
        // Create a parent GameObject to hold the room parts
        GameObject roomParent = new GameObject("GeneratedRoom");
        roomParent.tag = "Room";

        // Helper
        ProBuilderMesh CreateBox(string name, Vector3 position, Vector3 size, bool collider = true, bool plane = false, Material material = null)
        {
        
            ProBuilderMesh pbMesh = plane ? ShapeGenerator.GeneratePlane(PivotLocation.Center, width, depth, 1, 1, Axis.Down) : ShapeGenerator.GenerateCube(PivotLocation.Center, size);
            pbMesh.name = name;
            initMesh(pbMesh, true, material, roomParent);
            pbMesh.transform.localPosition = position;
            return pbMesh;

        }

        float floorThickness = 0.1f;

        float f = wallThickness * 2;

        // Floor
        CreateBox("Floor", new Vector3(0, -floorThickness * 0.5f, 0), new Vector3(width, floorThickness, depth), material: this.floorMaterial);
        
        //Ceiling
        ProBuilderMesh ceiling = CreateBox("Ceiling", new Vector3(0, height, 0), new Vector3(width, wallThickness, depth), collider: false, plane: true);
        ceiling.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        ceiling.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        
        //Walls
        CreateBox("Wall_Front", new Vector3(0, height / 2, depth / 2 - wallThickness / 2), new Vector3(width - f, height, wallThickness), material: this.wallMaterial);
        CreateBox("Wall_Back", new Vector3(0, height / 2, -depth / 2 + wallThickness / 2), new Vector3(width - f, height, wallThickness), material: this.wallMaterial);
        CreateBox("Wall_Left", new Vector3(-width / 2 + wallThickness / 2, height / 2, 0), new Vector3(wallThickness, height, depth), material: this.wallMaterial);
        CreateBox("Wall_Right", new Vector3(width / 2 - wallThickness / 2, height / 2, 0), new Vector3(wallThickness, height, depth), material: this.wallMaterial);
    
        //generate light probes
        LightProbeGroup lightProbeGroup = new GameObject().AddComponent<LightProbeGroup>();
        lightProbeGroup.name = "Light Probes";
        lightProbeGroup.transform.parent = roomParent.transform;
        List<Vector3> probes = new();
        for(float i = -width * 0.5f;i < width * 0.5f;i += probePerMeter.x)
        {
            for(float j = 0;j < height;j += probePerMeter.y)
            {
                for(float k = -depth * 0.5f;k < depth * 0.5f;k += probePerMeter.z)
                {
                
                    probes.Add(new Vector3(i + wallThickness,j,k + wallThickness));
                }
            }
        }
        lightProbeGroup.probePositions = probes.ToArray();
    }
}
#endif
