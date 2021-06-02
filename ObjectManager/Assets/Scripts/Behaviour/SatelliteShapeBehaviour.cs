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
        cosOffset = Vector3.right;
        sinOffset = Vector3.forward;
        cosOffset *= orbitRadius;
        sinOffset *= orbitRadius;
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
