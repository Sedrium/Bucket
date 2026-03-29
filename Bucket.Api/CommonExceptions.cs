namespace Backend_Test
{
    public class CommonExceptions
    {
        /// <summary>
        /// If the item doesn't match what we want to update
        /// </summary>
        public void IdDoesNotMatch()
        {
            throw new System.Exception("Id doesn't match item");
        }

        /// <summary>
        /// If an item doesn't exist
        /// </summary>
        public void ItemNotExists()
        {
            throw new System.Exception("Item does not exist");
        }
    }
}
