﻿using System.Collections;
using System.Reflection;
using System;
using Castle.Core.Logging;
using KojtoCAD.BlockItems.Interfaces;
using KojtoCAD.Properties;
using KojtoCAD.Utilities;
#if !bcad
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Exception = Autodesk.AutoCAD.Runtime.Exception;
#else
using Teigha.DatabaseServices;
using Teigha.Geometry;
using Teigha.Runtime;
using Bricscad.EditorInput;
using Application = Bricscad.ApplicationServices.Application;
using Exception = Teigha.Runtime.Exception;
#endif

[assembly: CommandClass(typeof(KojtoCAD.GraphicItems.SlottedHoleRound))]

namespace KojtoCAD.GraphicItems
{

    public class SlottedHoleRound
    {
        private readonly EditorHelper _editorHelper;
        private readonly DocumentHelper _documentHelper;
        private static ILogger _logger = NullLogger.Instance;
        private readonly IBlockDrawingProvider _blockDrawingProvider;

        public SlottedHoleRound()
        {
            _blockDrawingProvider = IoC.ContainerRegistrar.Container.Resolve<IBlockDrawingProvider>();
            _logger = IoC.ContainerRegistrar.Container.Resolve<ILogger>();
            _editorHelper = new EditorHelper(Application.DocumentManager.MdiActiveDocument);
            _documentHelper = new DocumentHelper(Application.DocumentManager.MdiActiveDocument);
        }

        /// <summary>
        /// Slotted Hole Round
        /// </summary>
        [CommandMethod("sho")]
        public void SlottedHoleRoundStart()
        {
            

            var keywords = new[] { "SingleAxis", "DoubleAxis" };
            var slottedHoleRoundType = _editorHelper.PromptForKeywordSelection("Select the type of slotted hole : ", keywords, false, keywords[0]);
            if (slottedHoleRoundType.Status != PromptStatus.OK)
            {
                return;
            }
            var basePointResult = _editorHelper.PromptForPoint("Pick insertion point : ");
            if (basePointResult.Status != PromptStatus.OK)
            {
                return;
            }

            var lengthResult = _editorHelper.PromptForDouble("Enter length : ", Settings.Default.SlottedHoleLength);
            if (lengthResult.Status != PromptStatus.OK)
            {
                return;
            }
            if (lengthResult.Value <= 0)
            {
                _logger.Info("Incorect User Input!");
                _editorHelper.WriteMessage("Length must be a Positive number.");
                return;
            }
            Settings.Default.SlottedHoleLength = lengthResult.Value;

            var widthResult = _editorHelper.PromptForDouble("Enter width : ", Settings.Default.SlottedHoleDiameter);
            if (widthResult.Status != PromptStatus.OK)
            {
                return;
            }
            if (widthResult.Value <= 0)
            {
                _logger.Info("Incorect User Input!");
                _editorHelper.WriteMessage("Width must be a Positive number");
                return;
            }
            if (slottedHoleRoundType.StringResult == keywords[0] && (lengthResult.Value - widthResult.Value) <= 0)
            {
                _editorHelper.WriteMessage("Improper ratio between length and width.");
                return;
            }
            Settings.Default.SlottedHoleDiameter = widthResult.Value;

            var angleResult = _editorHelper.PromptForDouble("Enter rotation angle : ",
                                                            Settings.Default.SlottedHoleRotationAngle);
            if (angleResult.Status != PromptStatus.OK)
            {
                return;
            }
            Settings.Default.SlottedHoleRotationAngle = angleResult.Value;
            Settings.Default.Save();

            switch (slottedHoleRoundType.StringResult)
            {
                case "SingleAxis":
                    this.DrawSlottedHoleRound(basePointResult.Value, lengthResult.Value - widthResult.Value, widthResult.Value / 2, angleResult.Value * Math.PI / 180, "SingleAxis");
                    break;
                default:
                    this.DrawSlottedHoleRound(basePointResult.Value, lengthResult.Value, widthResult.Value/2, angleResult.Value * Math.PI / 180,"DoubleAxis");
                    break;
            }
        }

        private void DrawSlottedHoleRound(Point3d basePoint, double length, double width, double angle, string slottedHoleType)
        {
            var dynamicBlockProperties = new Hashtable();

            dynamicBlockProperties.Add("Length", length);
            dynamicBlockProperties.Add("Width", width);
            dynamicBlockProperties.Add("Angle", angle);

            var dynamicBlockId = new ObjectId();
            var dynamicBlockFullName =
                _blockDrawingProvider.GetBlockFile(MethodBase.GetCurrentMethod().DeclaringType.Name+slottedHoleType);
            if (dynamicBlockFullName == null)
            {
                _editorHelper.WriteMessage("Dynamic block SlottedHoleRound"+slottedHoleType+".dwg does not exist.");
                return;
            }
            try
            {
                dynamicBlockId = _documentHelper.ImportDynamicBlockAndFillItsProperties(dynamicBlockFullName,
                                                                                        basePoint,
                                                                                        dynamicBlockProperties,
                                                                                        new Hashtable());
            }
            catch (Exception exception)
            {
                _logger.Error("Error importing SlottedHoleRound .", exception);
                _editorHelper.WriteMessage("Error importing SlottedHoleRound.");
                return;
            }

            using (var startTransaction = _documentHelper.Database.TransactionManager.StartTransaction())
            {
                var entity = (Entity)startTransaction.GetObject(dynamicBlockId, OpenMode.ForWrite);
                entity.TransformBy(_editorHelper.CurrentUcs);
                startTransaction.Commit();
            }
            _logger.Info(MethodBase.GetCurrentMethod().Name);
            }
    }
}
