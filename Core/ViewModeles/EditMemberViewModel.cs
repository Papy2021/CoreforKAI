namespace Core.ViewModeles
{
    public class EditMemberViewModel: CreateMemberViewModel
    {
        public int Id { set; get; }
        public string? CurrentPhotoPath { set; get; }
    }
}
