using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Multiplayer.PlayMode;
using UnityEngine;
using UnityEngine.UI;


public class fishDataController : MonoBehaviour 
{
    public FishData FishData;
    public bool isPlayer;
    public Camera mainCamera;

    private fishBehavior aiBehavior;

    private fishPlayerInput playerInput;
    private fishMovement movement;

    private Outline outline;

    public GameObject thisPrefab;
    private Rigidbody rb;

    public float selectableRadius = 200f;
    private List<fishDataController> highlightedFish = new List<fishDataController>();
    public LayerMask fishLayer;
    private fishDataController currentLookTarget;

    void Start()
    {
        playerInput = GetComponent<fishPlayerInput>();
        aiBehavior = GetComponent<fishBehavior>();
        movement = GetComponent<fishMovement>();
        outline = GetComponent<Outline>();
        rb = GetComponent<Rigidbody>();

        if (outline != null)
        {
            outline.enabled = false;
        }

        thisPrefab = this.gameObject;

        applyFishData();

        if (isPlayer)
        {
            SwitchManager.Instance.AssignPlayer(this);
            FishDiscoveryManager.Instance.Discover(FishData);
            SetPlayer();
        }
        else
        {
            SetAI();
        }
    }

    void Update()
    {
        if (!isPlayer) return;
        if (SwitchConfirmPopup.Instance != null && SwitchConfirmPopup.Instance.IsOpen) return;
        updateLookTarget();
    }

    void applyFishData()
    {
        if (FishData == null)
        {
            Debug.LogError("attach" + this.name + " FishData pls");
            return;
        }

        movement.acceleration    = FishData.acceleration;
        movement.turnSpeed       = FishData.turnSpeed;
        movement.maxSpeed        = FishData.maxSpeed;
        movement.drag              = FishData.slowingSpeed;
        movement.horizontalEnabled = FishData.horizontalEnabled;
        movement.verticalEnabled   = FishData.verticalEnabled;

        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col != null)
            col.radius = FishData.colliderRadius;

        /* the 20 lines below are AI generated*/
        Transform modelContainer = transform.Find("ModelContainer");
        TransformAbility transformAbility = GetComponent<TransformAbility>();

        if (modelContainer != null)
        {
            foreach (Transform child in modelContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (var prefab in FishData.modelPrefabs)
            {
                GameObject model = Instantiate(prefab, modelContainer);
                instantiateModel(model);

                if (transformAbility != null)
                    transformAbility.RegisterModel(model);
            }

        }

    }

    private void instantiateModel(GameObject model)
    {
        outline = model.GetComponent<Outline>();
        if (outline == null)
            outline = model.AddComponent<Outline>();
        outline.enabled = false;

    }

  
    private void updateLookTarget()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.SphereCast(ray, 1.5f, out RaycastHit hit, 100f))
        {
            fishDataController newTarget = hit.collider.GetComponentInParent<fishDataController>();

            if (newTarget != null && newTarget != this && newTarget.CompareTag("fish"))
            {
                if (currentLookTarget != null && currentLookTarget != newTarget)
                {
                    clearSelection(currentLookTarget);
                }

                currentLookTarget = newTarget;
                currentLookTarget.setHighlight(Color.white);
                return;
            }
        }

        if (currentLookTarget != null)
        {
            clearSelection(currentLookTarget);
            currentLookTarget = null;
        }
    }

    private Outline getActiveOutline()
    {
        Transform modelContainer = transform.Find("ModelContainer");
        if (modelContainer == null) return null;

        foreach (Transform child in modelContainer)
        {
            if (child.gameObject.activeSelf)
                return child.GetComponent<Outline>();
        }
        return null;
    }


    public void setHighlight(Color color)
    {
        outline = getActiveOutline();

        if (outline != null)
        {
            if (outline.enabled && outline.OutlineColor == Color.white && color != Color.white)
            {
                return;
            }
            outline.enabled = true;
            outline.OutlineColor = color;
            outline.OutlineWidth = 6;
        }
    }

    public void unhighlight()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void clearSelection(fishDataController fish)
    {
        if (highlightedFish.Contains(fish))
        {
            fish.setHighlight(Color.yellow);
        }
        else
        {
            fish.unhighlight();
        }
    }

    public void SetPlayer()
    {
        isPlayer = true;

        if (playerInput != null)
        {
            playerInput.enabled = true;
        }
        if (aiBehavior != null)
        {
            aiBehavior.disable();
        }

        PlayerFishTracker.Current = transform;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.isKinematic = false;

        SwitchManager.Instance.AssignPlayer(this);
    }

    public void SetAI()
    {
        isPlayer = false;

        if (playerInput != null)
        {
            playerInput.enabled = false;
        }
        if (aiBehavior != null)
        {
            aiBehavior.enable();
        }

        rb.interpolation = RigidbodyInterpolation.None;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

    }
}

