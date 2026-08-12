using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;

namespace PlotDigitizer.Core.Tests.Wpf.Controls
{
	/// <summary>
	/// The axis-limit input used on the range page. Its label doubles as the automation id the
	/// UI tests address it by, so both have to stay in step.
	/// </summary>
	[TestClass]
	public class AxLimTextBoxTests
	{
		private static TextBox InnerTextBox(AxLimTextBox control) => (TextBox)control.FindName("textBox");

		/// <summary>
		/// The inner text box binds through <c>RelativeSource AncestorType=UserControl</c>, which
		/// only resolves once the control has been through a layout pass.
		/// </summary>
		private static AxLimTextBox CreateArrangedControl(string label = "")
		{
			var control = new AxLimTextBox { Label = label };
			control.Measure(new Size(200, 100));
			control.Arrange(new Rect(0, 0, 200, 100));
			control.UpdateLayout();
			return control;
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Defaults_AreEmpty()
		{
			StaTestContext.Run(() =>
			{
				var control = new AxLimTextBox();

				Assert.AreEqual(string.Empty, control.Label, "label");
				Assert.AreEqual(string.Empty, control.Text, "text");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Text_BindsTwoWayByDefault()
		{
			var metadata = (FrameworkPropertyMetadata)AxLimTextBox.TextProperty.GetMetadata(typeof(AxLimTextBox));

			Assert.IsTrue(metadata.BindsTwoWayByDefault);
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Text_WhenSet_ReachesTheInnerTextBox()
		{
			StaTestContext.Run(() =>
			{
				var control = CreateArrangedControl();

				control.Text = "123";

				Assert.AreEqual("123", InnerTextBox(control).Text);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void InnerTextBox_TakesItsAutomationIdFromTheLabel()
		{
			StaTestContext.Run(() =>
			{
				var control = CreateArrangedControl("XMin");

				Assert.AreEqual("XMin", AutomationProperties.GetAutomationId(InnerTextBox(control)));
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void InnerTextBox_CarriesTheAxisLimitBehaviours()
		{
			StaTestContext.Run(() =>
			{
				var behaviors = Microsoft.Xaml.Behaviors.Interaction.GetBehaviors(InnerTextBox(CreateArrangedControl()));

				Assert.AreEqual(2, behaviors.Count, "behaviour count");
				Assert.IsTrue(behaviors.Any(b => b is SelectAllTextOnFocusBehavior), "select all on focus");
				Assert.IsTrue(behaviors.Any(b => b is UpdateTextOnKeyDown), "commit on Enter");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void InnerTextBox_ValidatesItsTextAsANumber()
		{
			StaTestContext.Run(() =>
			{
				var textBox = InnerTextBox(CreateArrangedControl());

				var binding = textBox.GetBindingExpression(TextBox.TextProperty).ParentBinding;

				Assert.AreEqual(1, binding.ValidationRules.Count, "validation rule count");
				Assert.IsInstanceOfType(binding.ValidationRules[0], typeof(NumberValidation));
			});
		}
	}
}
