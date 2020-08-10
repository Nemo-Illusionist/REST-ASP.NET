using System;

namespace Radilovsoft.Rest.Infrastructure.Test.Helpers
{
    public class TestDto
    {
        public Guid Id { get; set; }
        
        public int Order { get; set; }

        public string Name { get; set; }

        public DateTime CreatedUtc { get; set; }

        public TestDto SubTest { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is TestDto testDto)
            {
                return Id == testDto.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}