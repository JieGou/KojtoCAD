namespace KojtoCAD.Ui.Interfaces
{
    /// <summary>
    /// Ui生成接口
    /// </summary>
    public interface IUiGenerator
    {
        void GenerateUi(bool regenerateIfExists);
        void RegenerateUi();

        void RemoveUi();
    }
}
