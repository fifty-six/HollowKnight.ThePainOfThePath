using System.Collections;
using Modding;
using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using Object = UnityEngine.Object;

namespace ThePainOfThePath
{
    [UsedImplicitly]
    public class ThePainOfThePath : Mod, ITogglableMod
    {
        private static GameManager Gm => GameManager.instance;

        public static bool Backwards { private get; set; }

        private const float SAW = 1.362954f;

        private const string DOOR_NAME = "door_potp";

        public ThePainOfThePath() : base("The Pain of the Path") { }

        public override void Initialize()
        {
            On.GameManager.EnterHero += OnEnterHero;
            USceneManager.activeSceneChanged += SceneChanged;
            On.QuitToMenu.Start += OnQuitToMenu;
        }

        private static IEnumerator OnQuitToMenu(On.QuitToMenu.orig_Start orig, QuitToMenu self)
        {
            Backwards = false;
            yield return orig(self);
        }

        private static void OnEnterHero(On.GameManager.orig_EnterHero orig, GameManager self, bool additivegatesearch)
        {
            self.UpdateSceneName();

            if (self.sceneName != "White_Palace_20")
            {
                orig(self, additivegatesearch);
                return;
            }

            var go = new GameObject(DOOR_NAME);

            var tp = go.AddComponent<TransitionPoint>();
            tp.respawnMarker = go.AddComponent<HazardRespawnMarker>();
            tp.isADoor = true;
            tp.name = DOOR_NAME;

            go.transform.position = new Vector3(228.4f, 165.3f);

            orig(self, false);
        }

        private static void SceneChanged(Scene arg0, Scene arg1)
        {
            switch (arg1.name)
            {
                case "White_Palace_18" when Backwards:
                {
                    GameObject saw = GameObject.Find("saw_collection/wp_saw (4)");

                    GameObject topSaw = Object.Instantiate(saw);
                    topSaw.transform.SetPositionX(165f);
                    topSaw.transform.SetPositionY(30.5f);
                    topSaw.transform.localScale = new Vector3(SAW / 1.5f, SAW / 2, SAW);

                    GameObject botSaw = Object.Instantiate(saw);
                    botSaw.transform.SetPositionX(161.4f);
                    botSaw.transform.SetPositionY(21.4f);
                    botSaw.transform.localScale = new Vector3(SAW / 1.5f, SAW / 2, SAW);

                    goto case "Lever";
                }
                case "White_Palace_17" when Backwards:
                {
                    CreatePlane(new[]
                    {
                        new Vector3(72.9f, 29),
                        new Vector3(72.9f, 38f),
                        new Vector3(73, 29),
                        new Vector3(73, 38f)
                    }).AddComponent<OneWayWall>();

                    CreatePlane(new[]
                    {
                        new Vector3(83, 31.9f),
                        new Vector3(83, 32),
                        new Vector3(87, 31.9f),
                        new Vector3(87, 32f)
                    }, "KillMe").AddComponent<FlashOnHit>();

                    goto case "Lever";
                }

                case "White_Palace_20" when Backwards:
                {
                    CreatePlane(new[]
                    {
                        new Vector3(225.8f, 165.1f),
                        new Vector3(225.8f, 165.2f),
                        new Vector3(231, 165.1f),
                        new Vector3(231, 165.2f)
                    });

                    GameObject totem = Object.Instantiate(GameObject.Find("Soul Totem white_Infinte (2)"));
                    
                    totem.transform.position = new Vector3(228.7f, 168.2f, 0.1f);

                    var exit = GameObject.Find("saw_collection/wp_saw (64)").AddComponent<BetterTransitionPoint>();

                    exit.SceneName = "White_Palace_06";
                    exit.EntryGateName = "bot1";
                    
                    goto case "Lever";
                }

                case "White_Palace_06":
                {
                    var tp = CreatePlane(new[]
                    {
                        new Vector3(26.5f, 2.0f),
                        new Vector3(26.5f, 5.1f),
                        new Vector3(29, 2.0f),
                        new Vector3(29, 5.1f)
                    }, "tp", Color.black).AddComponent<BetterTransitionPoint>();

                    tp.SceneName = "White_Palace_20";
                    tp.EntryGateName = DOOR_NAME;
                    tp.SetBackwards = true;

                    if (Gm.entryGateName == "left1")
                    {
                        Backwards = false;
                    }

                    break;
                }

                case "Lever":
                {
                    SceneData sd = Gm.sceneData;

                    sd.SaveMyState(new PersistentBoolData
                    {
                        sceneName = "White_Palace_17",
                        id = "WP Lever",
                        activated = true
                    });

                    sd.SaveMyState(new PersistentBoolData
                    {
                        sceneName = "White_Palace_17",
                        id = "Collapser Small",
                        activated = true
                    });
                    break;
                }
            }
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private static GameObject CreatePlane(Vector3[] vert, string name = "Plane", Color? c = null)
        {
            var plane = new GameObject(name)
            {
                tag = "HeroWalkable",
                layer = 8
            };

            var meshFilter = plane.AddComponent<MeshFilter>();
            meshFilter.mesh = CreateMesh(vert);

            var renderer = plane.AddComponent<MeshRenderer>();
            renderer.material.shader = Shader.Find("Particles/Multiply");

            // Texture
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, c ?? Color.white);
            tex.Apply();

            // Renderer
            renderer.material.mainTexture = tex;
            renderer.material.color = Color.white;

            // Collider
            var col = plane.AddComponent<BoxCollider2D>();
            col.isTrigger = false;

            // Make it exist.
            plane.SetActive(true);

            return plane;
        }

        private static Mesh CreateMesh(Vector3[] vertices)
        {
            var m = new Mesh
            {
                name = "ScriptedMesh",
                vertices = vertices,
                uv = new Vector2[]
                {
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0)
                },
                triangles = new int[] {0, 1, 2, 1, 2, 3}
            };
            m.RecalculateNormals();
            return m;
        }

        public void Unload()
        {
            On.GameManager.EnterHero -= OnEnterHero;
            USceneManager.activeSceneChanged -= SceneChanged;
            On.QuitToMenu.Start -= OnQuitToMenu;
        }
    }
}