namespace Quarantine
{
	partial class Form1
	{
		/// <summary>
		/// Variable nécessaire au concepteur.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Nettoyage des ressources utilisées.
		/// </summary>
		/// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Code généré par le Concepteur Windows Form

		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{
			SuspendLayout();
			// 
			// Form1
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
			ClientSize = new System.Drawing.Size(675, 694);
			DoubleBuffered = true;
			Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			Name = "Form1";
			Text = "Quarantine";
			Load += Form1_Load;
			Paint += Form1_Paint;
			KeyDown += Form1_KeyDown;
			MouseDown += Form1_MouseDown;
			MouseMove += Form1_MouseMove;
			MouseUp += Form1_MouseUp;
			Resize += Form1_Resize;
			ResumeLayout(false);

		}

		#endregion
	}
}

