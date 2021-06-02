using UnityEngine;

class SatelliteShapeBehaviour : ShapeBehaviour
{
    private ShapeInstance focalShape;
    private float frequency;
    private Vector3 cosOffset, sinOffset;
    Vector3 previousPosition;

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
        previousPosition = shape.transform.localPosition;
    }

    public override bool GameUpdate(Shape shape)
    {
        if (focalShape.IsVaild)
        {
            float t = 2f * Mathf.PI * frequency * shape.Age;
            previousPosition = shape.transform.localPosition;
            shape.transform.localPosition =
                focalShape.Shape.transform.localPosition +
                cosOffset * Mathf.Cos(t) + sinOffset * Mathf.Sin(t);
            return true;
        }
        shape.AddBehaviour<MovementShapeBehaviour>().Velocity = (shape.transform.localPosition - previousPosition) / Time.deltaTime;
        return false;
    }

    public override void Load(GameDataReader reader)
    {
        frequency = reader.ReadFloat();
        cosOffset = reader.ReadVector3();
        sinOffset = reader.ReadVector3();
        previousPosition = reader.ReadVector3();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(frequency);
        writer.Write(cosOffset);
        writer.Write(sinOffset);
        writer.Write(previousPosition);
    }

    public override void Recycle()
    {
        ShapeBehaviourPool<SatelliteShapeBehaviour>.Reclaim(this);
    }
}
