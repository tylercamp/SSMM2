using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;

namespace SSMM2.Editors
{
	public partial class ParticleEditor : Form
	{
		public ParticleEditor()
		{
			InitializeComponent();
		}

		private void SafeSetTrackbar(TrackBar target, int value)
		{
			if (value > target.Maximum)
			{
				target.Value = target.Maximum;
				return;
			}

			if (value < target.Minimum)
			{
				target.Value = target.Minimum;
			}

			target.Value = value;
		}
		private void SafeSetTrackbar(TrackBar target, float value)
		{
			SafeSetTrackbar(target, (int)value);
		}

		public void SyncUIToParticleSystem(Effects.ParticleSystemV1 source)
		{
			Effects.ParticleSystemDescriptor desc = source.Descriptor;


			/* Creation Properties */
			SafeSetTrackbar(tbarEjectaDirection, desc.EjectaDirection);
			txtbxEjectaDirection.Text = desc.EjectaDirection.ToString();

			SafeSetTrackbar(tbarMaxEjectaDirectionVariance, desc.MaxEjectaDirectionVariance);
			txtbxMaxEjectaDirectionVariance.Text = desc.MaxEjectaDirectionVariance.ToString();

			SafeSetTrackbar(tbarStartSpeedMin, desc.StartSpeed.Min);
			txtbxStartSpeedMin.Text = desc.StartSpeed.Min.ToString();
			SafeSetTrackbar(tbarStartSpeedMax, desc.StartSpeed.Max);
			txtbxStartSpeedMax.Text = desc.StartSpeed.Max.ToString();

			SafeSetTrackbar(tbarStartParticleScaleMin, desc.StartParticleScale.Min);
			txtbxStartParticleScaleMin.Text = desc.StartParticleScale.Min.ToString();
			SafeSetTrackbar(tbarStartParticleScaleMax, desc.StartParticleScale.Max);
			txtbxStartParticleScaleMax.Text = desc.StartParticleScale.Max.ToString();

			SafeSetTrackbar(tbarParticleScaleVarianceMin, desc.ParticleScaleVariance.Min);
			txtbxParticleScaleVarianceMin.Text = desc.ParticleScaleVariance.Min.ToString();
			SafeSetTrackbar(tbarParticleScaleVarianceMax, desc.ParticleScaleVariance.Max);
			txtbxParticleScaleVarianceMax.Text = desc.ParticleScaleVariance.Max.ToString();

			SafeSetTrackbar(tbarParticleLifeMin, desc.ParticleLife.Min);
			txtbxParticleLifeMin.Text = desc.ParticleLife.Min.ToString();
			SafeSetTrackbar(tbarParticleLifeMax, desc.ParticleLife.Max);
			txtbxParticleLifeMax.Text = desc.ParticleLife.Max.ToString();

			/* Behavior */
			SafeSetTrackbar(tbarRadialAccelerationMin, desc.RadialAcceleration.Min);
			txtbxRadialAccelerationMin.Text = desc.ParticleLife.Min.ToString();
			SafeSetTrackbar(tbarRadialAccelerationMax, desc.RadialAcceleration.Max);
			txtbxRadialAccelerationMax.Text = desc.ParticleLife.Max.ToString();

			SafeSetTrackbar(tbarTangentialAccelerationMin, desc.TangentialAcceleration.Min);
			txtbxTangentialAccelerationMin.Text = desc.TangentialAcceleration.Min.ToString();
			SafeSetTrackbar(tbarTangentialAccelerationMax, desc.TangentialAcceleration.Max);
			txtbxTangentialAccelerationMax.Text = desc.TangentialAcceleration.Max.ToString();

			SafeSetTrackbar(tbarGravityStrengthFactorMin, desc.GravityStrengthFactor.Min);
			txtbxGravityStrengthFactorMin.Text = desc.GravityStrengthFactor.Min.ToString();
			SafeSetTrackbar(tbarGravityStrengthFactorMax, desc.GravityStrengthFactor.Max);
			txtbxGravityStrengthFactorMax.Text = desc.GravityStrengthFactor.Max.ToString();

			/* Physics */
			SafeSetTrackbar(tbarBounceFactorMin, desc.BounceFactor.Min);
			txtbxBounceFactorMin.Text = desc.BounceFactor.Min.ToString();
			SafeSetTrackbar(tbarBounceFactorMax, desc.BounceFactor.Max);
			txtbxBounceFactorMax.Text = desc.BounceFactor.Max.ToString();

			chbxInteractive.Checked = desc.Interactive;


			/* Appearance */
			if (desc.Blend == BlendState.Additive)
				radioColorAdditive.Checked = true;
			if (desc.Blend == BlendState.AlphaBlend)
				radioColorAlphaBlend.Checked = true;

			chbxAgeAlpha.Checked = desc.AlphaFromAge;

			lbColors.Items.Clear();
			foreach (Microsoft.Xna.Framework.Color color in desc.ColorRanges)
			{
				lbColors.Items.Add(color.R + ", " + color.G + ", " + color.G + ", " + color.A);
			}
		}

		public void SyncParticleSystemToUI(Effects.ParticleSystemV1 target)
		{
			Effects.ParticleSystemDescriptor desc = target.Descriptor;

			/* Creation Properties */
			float.TryParse(txtbxEjectaDirection.Text, out desc.EjectaDirection);
			
			float.TryParse(txtbxMaxEjectaDirectionVariance.Text, out desc.MaxEjectaDirectionVariance);
			
			float.TryParse(txtbxStartSpeedMin.Text, out desc.StartSpeed.Min);
			float.TryParse(txtbxStartSpeedMax.Text, out desc.StartSpeed.Max);

			float.TryParse(txtbxStartParticleScaleMin.Text, out desc.StartParticleScale.Min);
			float.TryParse(txtbxStartParticleScaleMax.Text, out desc.StartParticleScale.Max);

			float.TryParse(txtbxParticleScaleVarianceMin.Text, out desc.ParticleScaleVariance.Min);
			float.TryParse(txtbxParticleScaleVarianceMax.Text, out desc.ParticleScaleVariance.Max);

			float.TryParse(txtbxParticleLifeMin.Text, out desc.ParticleLife.Min);
			float.TryParse(txtbxParticleLifeMax.Text, out desc.ParticleLife.Max);


			/* Behavior */
			float.TryParse(txtbxRadialAccelerationMin.Text, out desc.RadialAcceleration.Min);
			float.TryParse(txtbxRadialAccelerationMax.Text, out desc.RadialAcceleration.Max);

			float.TryParse(txtbxTangentialAccelerationMin.Text, out desc.TangentialAcceleration.Min);
			float.TryParse(txtbxTangentialAccelerationMax.Text, out desc.TangentialAcceleration.Max);

			float.TryParse(txtbxGravityStrengthFactorMin.Text, out desc.GravityStrengthFactor.Min);
			float.TryParse(txtbxGravityStrengthFactorMax.Text, out desc.GravityStrengthFactor.Max);

			/* Physics */
			float.TryParse(txtbxBounceFactorMin.Text, out desc.BounceFactor.Min);
			float.TryParse(txtbxBounceFactorMin.Text, out desc.BounceFactor.Max);

			desc.Interactive = chbxInteractive.Checked;

			/* Appearance */
			if (radioColorAdditive.Checked)
				desc.Blend = BlendState.Additive;
			if (radioColorAlphaBlend.Checked)
				desc.Blend = BlendState.AlphaBlend;

			desc.AlphaFromAge = chbxAgeAlpha.Checked;

			desc.ColorRanges = new Microsoft.Xna.Framework.Color[lbColors.Items.Count];

			for (int i = 0; i < desc.ColorRanges.Length; i++ )
			{
				String currentColor = lbColors.Items[i] as String;
				String[] components = currentColor.Split(',');

				desc.ColorRanges[i].R = byte.Parse(components[0].Trim());
				desc.ColorRanges[i].G = byte.Parse(components[1].Trim());
				desc.ColorRanges[i].B = byte.Parse(components[2].Trim());
				desc.ColorRanges[i].A = byte.Parse(components[3].Trim());
			}
		}

		private void SyncTrackbarAndTextbox(object source, TrackBar trackbar, TextBox textbox)
		{
			if (source.GetType() == trackbar.GetType())
			{
				textbox.Text = trackbar.Value.ToString();
			}

			if (source.GetType() == textbox.GetType())
			{
				try
				{
					int newValue = (int)float.Parse(textbox.Text);
					if (newValue > trackbar.Maximum)
						trackbar.Value = trackbar.Maximum;

					else if (newValue < trackbar.Minimum)
						trackbar.Value = trackbar.Minimum;

					else
						trackbar.Value = newValue;
				}
				catch (Exception e)
				{
				}
			}
		}

		private void SyncMinMax(object source, TrackBar minTrackbar, TextBox minTextbox, TrackBar maxTrackbar, TextBox maxTextbox)
		{
			if (source == null || source == minTrackbar || source == minTextbox)
			{
				maxTrackbar.Value = minTrackbar.Value;
				maxTextbox.Text = minTextbox.Text;
			}

			if (source == maxTrackbar || source == maxTextbox)
			{
				minTrackbar.Value = maxTrackbar.Value;
				minTextbox.Text = maxTextbox.Text;
			}
		}

		#region Creation Properties Data Input Handlers

		private void tbarEjectaDirection_Scroll(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarEjectaDirection, txtbxEjectaDirection);
		}

		private void txtbxEjectaDirection_TextChanged(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarEjectaDirection, txtbxEjectaDirection);
		}

		private void tbarMaxEjectaDirectionVariance_Scroll(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarMaxEjectaDirectionVariance, txtbxMaxEjectaDirectionVariance);
		}

		private void txtbxMaxEjectaDirectionVariance_TextChanged(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarMaxEjectaDirectionVariance, txtbxMaxEjectaDirectionVariance);
		}

		private void tbarStartSpeedMin_Scroll(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarStartSpeedMin, txtbxStartSpeedMin);

			if (chbxStartSpeedSync.Checked)
				SyncMinMax(sender, tbarStartSpeedMin, txtbxStartSpeedMin, tbarStartSpeedMax, txtbxStartSpeedMax);
		}

		private void txtbxStartSpeedMin_TextChanged(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarStartSpeedMin, txtbxStartSpeedMin);

			if (chbxStartSpeedSync.Checked)
				SyncMinMax(sender, tbarStartSpeedMin, txtbxStartSpeedMin, tbarStartSpeedMax, txtbxStartSpeedMax);
		}

		private void tbarStartSpeedMax_Scroll(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarStartSpeedMax, txtbxStartSpeedMax);

			if (chbxStartSpeedSync.Checked)
				SyncMinMax(sender, tbarStartSpeedMin, txtbxStartSpeedMin, tbarStartSpeedMax, txtbxStartSpeedMax);
		}

		private void txtbxStartSpeedMax_TextChanged(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarStartSpeedMax, txtbxStartSpeedMax);

			if (chbxStartSpeedSync.Checked)
				SyncMinMax(sender, tbarStartSpeedMin, txtbxStartSpeedMin, tbarStartSpeedMax, txtbxStartSpeedMax);
		}

		private void tbarStartParticleScaleMin_Scroll(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarStartParticleScaleMin, txtbxStartParticleScaleMin);

			if (chbxStartSpeedSync.Checked)
				SyncMinMax(sender, tbarStartParticleScaleMin, txtbxStartParticleScaleMin, tbarStartParticleScaleMax, txtbxStartParticleScaleMax);
		}

		private void txtbxStartParticleScaleMin_TextChanged(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarStartParticleScaleMin, txtbxStartParticleScaleMin);

			if (chbxStartSpeedSync.Checked)
				SyncMinMax(sender, tbarStartParticleScaleMin, txtbxStartParticleScaleMin, tbarStartParticleScaleMax, txtbxStartParticleScaleMax);
		}

		private void tbarStartParticleScaleMax_Scroll(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarStartParticleScaleMax, txtbxStartParticleScaleMax);

			if (chbxStartSpeedSync.Checked)
				SyncMinMax(sender, tbarStartParticleScaleMin, txtbxStartParticleScaleMin, tbarStartParticleScaleMax, txtbxStartParticleScaleMax);
		}

		private void txtbxStartParticleScaleMax_TextChanged(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarStartParticleScaleMax, txtbxStartParticleScaleMax);

			if (chbxStartSpeedSync.Checked)
				SyncMinMax(sender, tbarStartParticleScaleMin, txtbxStartParticleScaleMin, tbarStartParticleScaleMax, txtbxStartParticleScaleMax);
		}

		private void tbarParticleScaleVarianceMin_Scroll(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarParticleScaleVarianceMin, txtbxParticleScaleVarianceMin);

			if (chbxStartSpeedSync.Checked)
				SyncMinMax(sender, tbarParticleScaleVarianceMin, txtbxParticleScaleVarianceMin, tbarParticleScaleVarianceMax, txtbxParticleScaleVarianceMax);
		}

		private void txtbxParticleScaleVarianceMin_TextChanged(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarParticleScaleVarianceMin, txtbxParticleScaleVarianceMin);

			if (chbxStartSpeedSync.Checked)
				SyncMinMax(sender, tbarParticleScaleVarianceMin, txtbxParticleScaleVarianceMin, tbarParticleScaleVarianceMax, txtbxParticleScaleVarianceMax);
		}

		private void tbarParticleScaleVarianceMax_Scroll(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarParticleScaleVarianceMax, txtbxParticleScaleVarianceMax);

			if (chbxStartSpeedSync.Checked)
				SyncMinMax(sender, tbarParticleScaleVarianceMin, txtbxParticleScaleVarianceMin, tbarParticleScaleVarianceMax, txtbxParticleScaleVarianceMax);
		}

		private void txtbxParticleScaleVarianceMax_TextChanged(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarParticleScaleVarianceMax, txtbxParticleScaleVarianceMax);

			if (chbxStartSpeedSync.Checked)
				SyncMinMax(sender, tbarParticleScaleVarianceMin, txtbxParticleScaleVarianceMin, tbarParticleScaleVarianceMax, txtbxParticleScaleVarianceMax);
		}

		private void tbarParticleLifeMin_Scroll(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarParticleLifeMin, txtbxParticleLifeMin);

			if (chbxStartSpeedSync.Checked)
				SyncMinMax(sender, tbarParticleLifeMin, txtbxParticleLifeMin, tbarParticleLifeMax, txtbxParticleLifeMax);
		}

		private void txtbxParticleLifeMin_TextChanged(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarParticleLifeMin, txtbxParticleLifeMin);

			if (chbxStartSpeedSync.Checked)
				SyncMinMax(sender, tbarParticleLifeMin, txtbxParticleLifeMin, tbarParticleLifeMax, txtbxParticleLifeMax);
		}

		private void tbarParticleLifeMax_Scroll(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarParticleLifeMax, txtbxParticleLifeMax);

			if (chbxStartSpeedSync.Checked)
				SyncMinMax(sender, tbarParticleLifeMin, txtbxParticleLifeMin, tbarParticleLifeMax, txtbxParticleLifeMax);
		}

		private void txtbxParticleLifeMax_TextChanged(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarParticleLifeMax, txtbxParticleLifeMax);

			if (chbxStartSpeedSync.Checked)
				SyncMinMax(sender, tbarParticleLifeMin, txtbxParticleLifeMin, tbarParticleLifeMax, txtbxParticleLifeMax);
		}

		private void chbxStartSpeedSync_CheckedChanged(object sender, EventArgs e)
		{
			SyncMinMax(null, tbarStartSpeedMin, txtbxStartSpeedMin, tbarStartSpeedMax, txtbxStartSpeedMax);
		}

		private void chbxStartParticleScaleSync_CheckedChanged(object sender, EventArgs e)
		{
			SyncMinMax(null, tbarStartParticleScaleMin, txtbxStartParticleScaleMin, tbarStartParticleScaleMax, txtbxStartParticleScaleMax);
		}

		private void chbxParticleScaleVarianceSync_CheckedChanged(object sender, EventArgs e)
		{
			SyncMinMax(null, tbarParticleScaleVarianceMin, txtbxParticleScaleVarianceMin, tbarParticleScaleVarianceMax, txtbxParticleScaleVarianceMax);
		}

		private void chbxParticleLifeSync_CheckedChanged(object sender, EventArgs e)
		{
			SyncMinMax(null, tbarParticleLifeMin, txtbxParticleLifeMin, tbarParticleLifeMax, txtbxParticleLifeMax);
		}

		#endregion

		#region Behavior Data Input Handlers

		private void tbarRadialAccelerationMin_Scroll(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarRadialAccelerationMin, txtbxRadialAccelerationMin);

			if (chbxRadialAccelerationSync.Checked)
				SyncMinMax(sender, tbarRadialAccelerationMin, txtbxRadialAccelerationMin, tbarRadialAccelerationMax, txtbxRadialAccelerationMax);
		}

		private void txtbxRadialAccelerationMin_TextChanged(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarRadialAccelerationMin, txtbxRadialAccelerationMin);

			if (chbxRadialAccelerationSync.Checked)
				SyncMinMax(sender, tbarRadialAccelerationMin, txtbxRadialAccelerationMin, tbarRadialAccelerationMax, txtbxRadialAccelerationMax);
		}

		private void tbarRadialAccelerationMax_Scroll(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarRadialAccelerationMax, txtbxRadialAccelerationMax);

			if (chbxRadialAccelerationSync.Checked)
				SyncMinMax(sender, tbarRadialAccelerationMin, txtbxRadialAccelerationMin, tbarRadialAccelerationMax, txtbxRadialAccelerationMax);
		}

		private void txtbxRadialAccelerationMax_TextChanged(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarRadialAccelerationMax, txtbxRadialAccelerationMax);

			if (chbxRadialAccelerationSync.Checked)
				SyncMinMax(sender, tbarRadialAccelerationMin, txtbxRadialAccelerationMin, tbarRadialAccelerationMax, txtbxRadialAccelerationMax);
		}

		private void tbarTangentialAccelerationMin_Scroll(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarTangentialAccelerationMin, txtbxTangentialAccelerationMin);

			if (chbxTangentialAccelerationSync.Checked)
				SyncMinMax(sender, tbarTangentialAccelerationMin, txtbxTangentialAccelerationMin, tbarTangentialAccelerationMax, txtbxTangentialAccelerationMax);
		}

		private void txtbxTangentialAccelerationMin_TextChanged(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarTangentialAccelerationMin, txtbxTangentialAccelerationMin);

			if (chbxTangentialAccelerationSync.Checked)
				SyncMinMax(sender, tbarTangentialAccelerationMin, txtbxTangentialAccelerationMin, tbarTangentialAccelerationMax, txtbxTangentialAccelerationMax);
		}

		private void tbarTangentialAccelerationMax_Scroll(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarTangentialAccelerationMax, txtbxTangentialAccelerationMax);

			if (chbxTangentialAccelerationSync.Checked)
				SyncMinMax(sender, tbarTangentialAccelerationMin, txtbxTangentialAccelerationMin, tbarTangentialAccelerationMax, txtbxTangentialAccelerationMax);
		}

		private void txtbxTangentialAccelerationMax_TextChanged(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarTangentialAccelerationMax, txtbxTangentialAccelerationMax);

			if (chbxTangentialAccelerationSync.Checked)
				SyncMinMax(sender, tbarTangentialAccelerationMin, txtbxTangentialAccelerationMin, tbarTangentialAccelerationMax, txtbxTangentialAccelerationMax);
		}

		private void tbarGravityStrengthFactorMin_Scroll(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarGravityStrengthFactorMin, txtbxGravityStrengthFactorMin);

			if (chbxGravityStrengthFactorSync.Checked)
				SyncMinMax(sender, tbarGravityStrengthFactorMin, txtbxGravityStrengthFactorMin, tbarGravityStrengthFactorMax, txtbxGravityStrengthFactorMax);
		}

		private void txtbxGravityStrengthFactorMin_TextChanged(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarGravityStrengthFactorMin, txtbxGravityStrengthFactorMin);

			if (chbxGravityStrengthFactorSync.Checked)
				SyncMinMax(sender, tbarGravityStrengthFactorMin, txtbxGravityStrengthFactorMin, tbarGravityStrengthFactorMax, txtbxGravityStrengthFactorMax);
		}

		private void tbarGravityStrengthFactorMax_Scroll(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarGravityStrengthFactorMax, txtbxGravityStrengthFactorMax);

			if (chbxGravityStrengthFactorSync.Checked)
				SyncMinMax(sender, tbarGravityStrengthFactorMin, txtbxGravityStrengthFactorMin, tbarGravityStrengthFactorMax, txtbxGravityStrengthFactorMax);
		}

		private void txtbxGravityStrengthFactorMax_TextChanged(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarGravityStrengthFactorMax, txtbxGravityStrengthFactorMax);

			if (chbxGravityStrengthFactorSync.Checked)
				SyncMinMax(sender, tbarGravityStrengthFactorMin, txtbxGravityStrengthFactorMin, tbarGravityStrengthFactorMax, txtbxGravityStrengthFactorMax);
		}

		private void chbxRadialAccelerationSync_CheckedChanged(object sender, EventArgs e)
		{
			if (chbxRadialAccelerationSync.Checked)
				SyncMinMax(null, tbarRadialAccelerationMin, txtbxRadialAccelerationMin, tbarRadialAccelerationMax, txtbxRadialAccelerationMax);
		}

		private void chbxTangentialAccelerationSync_CheckedChanged(object sender, EventArgs e)
		{
			if (chbxTangentialAccelerationSync.Checked)
				SyncMinMax(null, tbarTangentialAccelerationMin, txtbxTangentialAccelerationMin, tbarTangentialAccelerationMax, txtbxTangentialAccelerationMax);
		}

		private void chbxGravityStrengthFactorSync_CheckedChanged(object sender, EventArgs e)
		{
			if (chbxGravityStrengthFactorSync.Checked)
				SyncMinMax(null, tbarGravityStrengthFactorMin, txtbxGravityStrengthFactorMin, tbarGravityStrengthFactorMax, txtbxGravityStrengthFactorMax);
		}

		#endregion

		#region Physics Data Input Handlers

		private void chbxInteractive_CheckedChanged(object sender, EventArgs e)
		{
			
		}

		private void tbarBounceFactorMin_Scroll(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarBounceFactorMin, txtbxBounceFactorMin);

			if (chbxBounceFactorSync.Checked)
				SyncMinMax(null, tbarBounceFactorMin, txtbxBounceFactorMin, tbarBounceFactorMax, txtbxBounceFactorMax);
		}

		private void txtbxBounceFactorMin_TextChanged(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarBounceFactorMin, txtbxBounceFactorMin);

			if (chbxBounceFactorSync.Checked)
				SyncMinMax(null, tbarBounceFactorMin, txtbxBounceFactorMin, tbarBounceFactorMax, txtbxBounceFactorMax);
		}

		private void tbarBounceFactorMax_Scroll(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarBounceFactorMax, txtbxBounceFactorMax);

			if (chbxBounceFactorSync.Checked)
				SyncMinMax(null, tbarBounceFactorMin, txtbxBounceFactorMin, tbarBounceFactorMax, txtbxBounceFactorMax);
		}

		private void txtbxBounceFactorMax_TextChanged(object sender, EventArgs e)
		{
			SyncTrackbarAndTextbox(sender, tbarBounceFactorMax, txtbxBounceFactorMax);

			if (chbxBounceFactorSync.Checked)
				SyncMinMax(null, tbarBounceFactorMin, txtbxBounceFactorMin, tbarBounceFactorMax, txtbxBounceFactorMax);
		}

		private void chbxBounceFactorSync_CheckedChanged(object sender, EventArgs e)
		{
			if (chbxBounceFactorSync.Checked)
				SyncMinMax(null, tbarBounceFactorMin, txtbxBounceFactorMin, tbarBounceFactorMax, txtbxBounceFactorMax);
		}

		#endregion

		#region Color Data Input Handlers

		private void btnAddColor_Click(object sender, EventArgs e)
		{
			lbColors.Items.Add("255, 255, 255, 255");
		}

		private void btnRemoveColor_Click(object sender, EventArgs e)
		{
			if (lbColors.SelectedIndex != -1)
				lbColors.Items.RemoveAt(lbColors.SelectedIndex);

			if (lbColors.Items.Count == 0)
				lbColors.Items.Add("255, 255, 255, 255");
		}

		private void btnClearColors_Click(object sender, EventArgs e)
		{
			lbColors.Items.Clear();
			lbColors.Items.Add("255, 255, 255, 255");
		}

		private void btnColorUp_Click(object sender, EventArgs e)
		{
			if (lbColors.SelectedIndex <= 0)
				return;

			String currentValue, previousValue;
			currentValue = lbColors.Items[lbColors.SelectedIndex] as String;
			previousValue = lbColors.Items[lbColors.SelectedIndex - 1] as String;

			lbColors.Items[lbColors.SelectedIndex] = previousValue;
			lbColors.Items[lbColors.SelectedIndex - 1] = currentValue;
		}

		private void btnColorDown_Click(object sender, EventArgs e)
		{
			if (lbColors.SelectedIndex == -1)
				return;
			if (lbColors.SelectedIndex >= lbColors.Items.Count - 1)
				return;

			String currentValue, nextValue;
			currentValue = lbColors.Items[lbColors.SelectedIndex] as String;
			nextValue = lbColors.Items[lbColors.SelectedIndex + 1] as String;

			lbColors.Items[lbColors.SelectedIndex] = nextValue;
			lbColors.Items[lbColors.SelectedIndex + 1] = currentValue;
		}

		private String ModifyColorIndex(String color, int index, int newValue)
		{
			String[] components = color.Split(',');

			for (int i = 0; i < components.Length; i++)
			{
				components[i] = components[i].Trim();
			}

			components[index] = newValue.ToString();

			String newColor = "";
			foreach (String str in components)
			{
				newColor += str + ", ";
			}

			newColor = newColor.Substring(0, newColor.Length - 2);

			return newColor;
		}

		private void tbarColorRed_Scroll(object sender, EventArgs e)
		{
			if (lbColors.SelectedIndex != -1)
				lbColors.Items[lbColors.SelectedIndex] = ModifyColorIndex(lbColors.SelectedItem as String, 0, tbarColorRed.Value);
		}

		private void tbarColorGreen_Scroll(object sender, EventArgs e)
		{
			if (lbColors.SelectedIndex != -1)
				lbColors.Items[lbColors.SelectedIndex] = ModifyColorIndex(lbColors.SelectedItem as String, 1, tbarColorGreen.Value);
		}

		private void tbarColorBlue_Scroll(object sender, EventArgs e)
		{
			if (lbColors.SelectedIndex != -1)
				lbColors.Items[lbColors.SelectedIndex] = ModifyColorIndex(lbColors.SelectedItem as String, 2, tbarColorBlue.Value);
		}

		private void tbarColorAlpha_Scroll(object sender, EventArgs e)
		{
			if (lbColors.SelectedIndex != -1)
				lbColors.Items[lbColors.SelectedIndex] = ModifyColorIndex(lbColors.SelectedItem as String, 3, tbarColorAlpha.Value);
		}

		private void lbColors_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lbColors.SelectedIndex == -1)
			{
				//tbarColorGreen.Enabled = false;
				//tbarColorRed.Enabled = false;
				//tbarColorBlue.Enabled = false;
				//tbarColorAlpha.Enabled = false;
			}
			else
			{
				tbarColorRed.Enabled = true;
				tbarColorGreen.Enabled = true;
				tbarColorBlue.Enabled = true;
				tbarColorAlpha.Enabled = true;

				String[] channels = (lbColors.Items[lbColors.SelectedIndex] as String).Split(',');
				for (int i = 0; i < channels.Length; i++)
				{
					channels[i] = channels[i].Trim();
				}

				tbarColorRed.Value = int.Parse(channels[0]);
				tbarColorGreen.Value = int.Parse(channels[1]);
				tbarColorBlue.Value = int.Parse(channels[2]);
				tbarColorAlpha.Value = int.Parse(channels[3]);

                picboxColorPreview.BackColor = Color.FromArgb(tbarColorRed.Value, tbarColorGreen.Value, tbarColorBlue.Value);
			}
		} 

		#endregion
	}
}
