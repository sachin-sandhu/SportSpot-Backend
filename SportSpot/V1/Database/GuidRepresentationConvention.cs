using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;

namespace SportSpot.V1.Database
{
    public class GuidRepresentationConvention : ConventionBase, IMemberMapConvention
    {
        public void Apply(BsonMemberMap memberMap)
        {
            if (memberMap.MemberType == typeof(Guid))
            {
                memberMap.SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
            }
            else if (memberMap.MemberType == typeof(Guid?))
            {
                memberMap.SetSerializer(new NullableSerializer<Guid>(new GuidSerializer(GuidRepresentation.Standard)));
            }else if (memberMap.MemberType == typeof(List<Guid>))
            {
                memberMap.SetSerializer(new EnumerableInterfaceImplementerSerializer<List<Guid>>(new GuidSerializer(GuidRepresentation.Standard)));
            }
        }
    }
}
