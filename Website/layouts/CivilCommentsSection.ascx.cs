using Sitecore.Data.Items;
using System;
using System.Linq;
using System.Web.UI.WebControls;
using Literal = System.Web.UI.WebControls.Literal;

namespace Website.layouts
{
    public partial class CivilCommentsSection : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var item = Sitecore.Context.Item;
            DisplayComments();
        }

        public void DisplayComments()
        {
            var dataSource = GetDataSourceItem();
            if (dataSource == null) return;

            var commentsFolder = dataSource.Axes.GetDescendants().FirstOrDefault(x => x.TemplateName == "Comments Folder");
            if (commentsFolder == null) return;
            var comments =
                commentsFolder.Axes.GetDescendants()
                    .Where(x => x.TemplateName == "Comment")
                    .OrderByDescending(x => x.Created);

            CommentsRepeater.DataSource = comments;
            CommentsRepeater.DataBind();
        }

        protected void SubmitBtn_OnClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public Item GetDataSourceItem()
        {
            var sublayout = this.Parent as Sitecore.Web.UI.WebControls.Sublayout;
            if (sublayout != null)
            {
                Guid dataSourceId;
                Sitecore.Data.Items.Item dataSource;
                if (Guid.TryParse(sublayout.DataSource, out dataSourceId))
                {
                    dataSource = Sitecore.Context.Database.GetItem(new Sitecore.Data.ID(dataSourceId));
                }
                else
                {
                    dataSource = Sitecore.Context.Database.GetItem(sublayout.DataSource);
                }
                return dataSource;
            }
            return null;
        }

        protected void CommentsRepeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Sitecore.Data.Items.Item item = e.Item.DataItem as Sitecore.Data.Items.Item;
            Literal litUsername = (Literal)e.Item.FindControl("CommentUserName");
            Literal litDate = (Literal)e.Item.FindControl("CommentDate");
            Literal litBody = (Literal)e.Item.FindControl("CommentBody");

            litUsername.Text = item.Fields["Username"].Value.Replace("sitecore\\", "");
            litBody.Text = item.Fields["Comment"].Value;
            litDate.Text = item.Created.ToString("g");

        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            var comment = Comment.Text;

            var dataSource = GetDataSourceItem();


        }
    }
}