using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GK;
using System;

public class RockObjectDetector : MonoBehaviour
{
    public BucketRocks manager;
    public List<GameObject> terrains; // Change to terrain list
    private double timecreated = 0.0;
    private Vector3 pos_last_collision = Vector3.zero;

    private void Start()
    {
        timecreated = Time.timeAsDouble;
        pos_last_collision = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (terrains.Contains(collision.gameObject))
        {
            pos_last_collision = transform.position;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (terrains.Contains(collision.gameObject))
        {
            pos_last_collision = transform.position;
        }
    }

    private void FixedUpdate()
    {
        if (Time.timeAsDouble - timecreated > 1.5)
        {
            var rigidbody = GetComponent<Rigidbody>();
            var velocity = rigidbody.velocity.sqrMagnitude;
            if (velocity < 0.2 && Vector3.Distance(transform.position, pos_last_collision) < 0.1)
            {
                manager.OnRockTerrainCollision(this.gameObject);
            }
        }
        if (this.transform.position.y - pos_last_collision.y < -1.0)
        {
            manager.OnRockTerrainCollision(this.gameObject);
        }
    }
}

public class BucketRocks : MonoBehaviour
{
    public GameObject rockPrefab;
    public List<GameObject> terrains = new List<GameObject>(); // Change to terrain list

    private List<GameObject> rocks;
    private ConvexHullCalculator calc;
    private float particle_volume;
    private double last_created_time = 0.0;

    private void Start()
    {
        calc = new ConvexHullCalculator();
        rocks = new List<GameObject>();
        particle_volume = (float)(4.0 / 3.0 * Math.PI * Math.Pow(SoilParticleSettings.instance.particleVisualRadius, 3));
        last_created_time = Time.timeAsDouble;
    }

    public void AddTerrain(GameObject terrain)
    {
        terrains.Add(terrain);
    }

    private void CreateRock(Vector3 point)
    {
        var rock = Instantiate(rockPrefab);

        rock.transform.SetParent(transform.root, false);
        rock.transform.position = point;
        rock.AddComponent<RockObjectDetector>();
        rock.GetComponent<RockObjectDetector>().manager = this;
        rock.GetComponent<RockObjectDetector>().terrains = terrains;

        var verts = new List<Vector3>();
        var tris = new List<int>();
        var normals = new List<Vector3>();
        var points = new List<Vector3>();

        points.Clear();

        for (int i = 0; i < 100; i++)
        {
            points.Add(UnityEngine.Random.insideUnitSphere * SoilParticleSettings.instance.particleVisualRadius);
        }

        calc.GenerateHull(points, true, ref verts, ref tris, ref normals);

        var mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetNormals(normals);
        rock.GetComponent<MeshFilter>().sharedMesh = mesh;

        rocks.Add(rock);
    }

    private void OnCollisionStay(Collision other)
    {
        if (SoilParticleSettings.instance == null || !SoilParticleSettings.instance.enable) return;

        if (terrains == null || terrains.Count == 0)
        {
            Debug.LogError("No terrains are set!");
            return;
        }

        if (terrains.Contains(other.gameObject) && Time.timeAsDouble - last_created_time > 0.05)
        {
            var point = other.GetContact(0).point;
            SoilParticleSettings.ModifyTerrain(point, -particle_volume);
            CreateRock(point);
            last_created_time = Time.timeAsDouble;
        }
    }

    public void OnRockTerrainCollision(GameObject rock)
    {
        if (Vector3.Distance(transform.position, rock.transform.position) > 2.0)
        {
            SoilParticleSettings.ModifyTerrain(rock.transform.position, particle_volume);
            Destroy(rock);
            rocks.Remove(rock);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (SoilParticleSettings.instance == null || !SoilParticleSettings.instance.enable) return;

        for (var i = 0; i < rocks.Count; i++)
        {
            var rock1 = rocks[i];
            if (rock1 == null)
            {
                Debug.LogWarning("Rock1 is null!");
                continue;
            }

            var repulvector = new Vector3();
            for (var j = 0; j < rocks.Count; j++)
            {
                var rock2 = rocks[j];
                if (rock2 == null)
                {
                    Debug.LogWarning("Rock2 is null!");
                    continue;
                }

                float dist = Vector3.Distance(rock1.transform.position, rock2.transform.position);
                if (dist < SoilParticleSettings.instance.partileStickDistance)
                {
                    repulvector += rock1.transform.position - rock2.transform.position;
                }
            }
            repulvector.Normalize();
            rock1.GetComponent<Rigidbody>().AddForce(-repulvector * SoilParticleSettings.instance.stickForce);
        }
    }
}