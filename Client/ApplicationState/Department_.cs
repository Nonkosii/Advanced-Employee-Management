namespace Client.ApplicationState
{
    public class Department_
    {
        public Action? GeneralDepart { get; set; }
        public bool ShowGeneralDepartment { get; set; }

        public void GeneralDepartment()
        {
            ResetAllDepartment();
            ShowGeneralDepartment = true;
            GeneralDepart?.Invoke();
        }

        private void ResetAllDepartment()
        {
            ShowGeneralDepartment = false;
        }
    }
}
