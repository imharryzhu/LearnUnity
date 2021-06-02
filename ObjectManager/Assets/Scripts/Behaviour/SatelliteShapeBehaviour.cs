using UnityEngine;

class SatelliteShapeBehaviour : ShapeBehaviour
{
    private Shape focalShape;
    private float frequency;
    private Vector3 cosOffset, sinOffset;

    public override ShapeBehaviourType BehaviourType => ShapeBehaviourType.Satellite;

    public void Initialize(Shape shape, Shape focalShape, 
        float orbitRadius, float oribtFrequency)
    {
        this.focalShape = focalShape;
        this.frequency = oribtFrequency;
        Vector3 orbitAxis = Random.onUnitSphere;
        do
        {
            cosOffset = Vector3.Cross(orbitAxis, Random.onUnitSphere).normalized;
        } while (cosOffset.sqrMagnitude < 0.1f); // 避免值过小导致数值为0
        sinOffset = Vector3.Cross(cosOffset, orbitAxis);
        cosOffset *= orbitRadius;
        sinOffset *= orbitRadius;

        shape.AddBehaviour<RotationShapeBehaviour>().AngularVelocity =
            -360f * frequency * 
            shape.transform.InverseTransformDirection(orbitAxis);

        GameUpdate(shape);
    }

    public override void GameUpdate(Shape shape)
    {
        float t = 2f * Mathf.PI * frequency * shape.Age;
        shape.transform.localPosition =
            focalShape.transform.localPosition +
            cosOffset * Mathf.Cos(t) + sinOffset * Mathf.Sin(t);
    }

    public override void Load(GameDataReader reader)
    {
    }

    public override void Save(GameDataWriter writer)
    {
    }

    public override void Recycle()
    {
        ShapeBehaviourPool<SatelliteShapeBehaviour>.Reclaim(this);
    }
}
