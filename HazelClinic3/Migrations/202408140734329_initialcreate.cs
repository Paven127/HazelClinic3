namespace HazelClinic3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialcreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuctionItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Image = c.Binary(),
                        Event_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.Event_Id)
                .Index(t => t.Event_Id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventName = c.String(nullable: false, maxLength: 100),
                        EventDate = c.DateTime(nullable: false),
                        EventTime = c.Time(nullable: false, precision: 7),
                        LimitOfAttendees = c.Int(nullable: false),
                        EventPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ArePetsAllowed = c.Boolean(nullable: false),
                        EventLocation = c.String(nullable: false, maxLength: 200),
                        Image = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EventDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        FileContent = c.Binary(),
                        Event_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.Event_Id)
                .Index(t => t.Event_Id);
            
            CreateTable(
                "dbo.DeclinedVolunteers",
                c => new
                    {
                        VolunteerId = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false),
                        Surname = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        CellNo = c.String(nullable: false, maxLength: 10),
                        EmergencyContactName = c.String(nullable: false),
                        EmergencyContactCellNo = c.String(nullable: false, maxLength: 10),
                        Experience = c.String(),
                        Availability = c.String(nullable: false),
                        VolunteerType = c.String(nullable: false),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.VolunteerId);
            
            CreateTable(
                "dbo.EventRegs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventId = c.Int(nullable: false),
                        FullName = c.String(nullable: false, maxLength: 100),
                        ContactNumber = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        IsNotRefundable = c.Boolean(nullable: false),
                        Quantity = c.Int(nullable: false),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TicketNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        RatingID = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        Rate = c.Int(nullable: false),
                        Note = c.String(),
                        promocode = c.String(),
                    })
                .PrimaryKey(t => t.RatingID);
            
            CreateTable(
                "dbo.ReturnPolicies",
                c => new
                    {
                        ReturnRequestID = c.Int(nullable: false, identity: true),
                        AdoptionID = c.Int(nullable: false),
                        ReturnDate = c.DateTime(nullable: false),
                        ScheduledReturnDate = c.DateTime(nullable: false),
                        ReturnStatus = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.ReturnRequestID);
            
            CreateTable(
                "dbo.Volunteers",
                c => new
                    {
                        VolunteerId = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false),
                        Surname = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        CellNo = c.String(nullable: false, maxLength: 10),
                        EmergencyContactName = c.String(nullable: false),
                        EmergencyContactCellNo = c.String(nullable: false, maxLength: 10),
                        Experience = c.String(),
                        Availability = c.String(nullable: false),
                        VolunteerType = c.String(nullable: false),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.VolunteerId);
            
            AddColumn("dbo.AdoptionRequests", "SubmittedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Appointments", "PromoCode", c => c.String());
            AddColumn("dbo.Bookings", "PromoCode", c => c.String());
            AddColumn("dbo.OngoingDrivers", "RescheduleCount", c => c.Int(nullable: false));
            AddColumn("dbo.User1", "Address", c => c.String(nullable: false));
            AddColumn("dbo.User1", "PostalCode", c => c.String(nullable: false));
            AlterColumn("dbo.AdoptionRequests", "AdopterFName", c => c.String());
            AlterColumn("dbo.AdoptionRequests", "AdopterNo", c => c.String());
            AlterColumn("dbo.AdoptionRequests", "AdopterEmail", c => c.String());
            AlterColumn("dbo.AdoptionRequests", "Address", c => c.String());
            AlterColumn("dbo.AdoptionRequests", "InspectionDate", c => c.DateTime());
            AlterColumn("dbo.Bookings", "Address", c => c.String());
            AlterColumn("dbo.Bookings", "CityPostalCode", c => c.String());
            AlterColumn("dbo.OngoingDrivers", "AdopterFName", c => c.String());
            AlterColumn("dbo.OngoingDrivers", "AdopterNo", c => c.String());
            AlterColumn("dbo.OngoingDrivers", "AdopterEmail", c => c.String());
            AlterColumn("dbo.OngoingDrivers", "Address", c => c.String());
            DropColumn("dbo.Bookings", "Message");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Bookings", "Message", c => c.String());
            DropForeignKey("dbo.EventRegs", "EventId", "dbo.Events");
            DropForeignKey("dbo.AuctionItems", "Event_Id", "dbo.Events");
            DropForeignKey("dbo.EventDocuments", "Event_Id", "dbo.Events");
            DropIndex("dbo.EventRegs", new[] { "EventId" });
            DropIndex("dbo.EventDocuments", new[] { "Event_Id" });
            DropIndex("dbo.AuctionItems", new[] { "Event_Id" });
            AlterColumn("dbo.OngoingDrivers", "Address", c => c.String(nullable: false));
            AlterColumn("dbo.OngoingDrivers", "AdopterEmail", c => c.String(nullable: false));
            AlterColumn("dbo.OngoingDrivers", "AdopterNo", c => c.String(nullable: false));
            AlterColumn("dbo.OngoingDrivers", "AdopterFName", c => c.String(nullable: false));
            AlterColumn("dbo.Bookings", "CityPostalCode", c => c.String(nullable: false));
            AlterColumn("dbo.Bookings", "Address", c => c.String(nullable: false));
            AlterColumn("dbo.AdoptionRequests", "InspectionDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AdoptionRequests", "Address", c => c.String(nullable: false));
            AlterColumn("dbo.AdoptionRequests", "AdopterEmail", c => c.String(nullable: false));
            AlterColumn("dbo.AdoptionRequests", "AdopterNo", c => c.String(nullable: false));
            AlterColumn("dbo.AdoptionRequests", "AdopterFName", c => c.String(nullable: false));
            DropColumn("dbo.User1", "PostalCode");
            DropColumn("dbo.User1", "Address");
            DropColumn("dbo.OngoingDrivers", "RescheduleCount");
            DropColumn("dbo.Bookings", "PromoCode");
            DropColumn("dbo.Appointments", "PromoCode");
            DropColumn("dbo.AdoptionRequests", "SubmittedDate");
            DropTable("dbo.Volunteers");
            DropTable("dbo.ReturnPolicies");
            DropTable("dbo.Ratings");
            DropTable("dbo.EventRegs");
            DropTable("dbo.DeclinedVolunteers");
            DropTable("dbo.EventDocuments");
            DropTable("dbo.Events");
            DropTable("dbo.AuctionItems");
        }
    }
}
