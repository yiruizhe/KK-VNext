using KK.DomainCommons;
using Listening.Domain.Events;
using Listening.Domain.Subtitles;
using Listening.Domain.ValueObjects;

namespace Listening.Domain.Entities;

public class Episode : AggregateRootEntity
{
    private Episode() { }

    public int SequenceNumber { get; private set; }//序号

    public string Name { get; private set; }//标题

    public string AlbumId { get; private set; }//专辑ID

    public Uri AudioUrl { get; private set; }//音频路径

    public double DurationInSecond { get; private set; }//音频秒数

    public string SubTitle { get; private set; }// 字幕

    public string SubtitleType { get; set; }//字幕类型

    public bool IsVisible { get; private set; }//用户是否可见

    public Episode ChangeSequenceNumber(int value)
    {
        this.SequenceNumber = value;
        this.AddDomainEventIfAbsent(new EpisodeUpdateEvent(this));
        return this;
    }

    public Episode ChangeName(string name)
    {
        this.Name = name;
        this.AddDomainEventIfAbsent(new EpisodeUpdateEvent(this));
        return this;
    }

    public Episode Hide()
    {
        IsVisible = false;
        this.AddDomainEventIfAbsent(new EpisodeUpdateEvent(this));
        return this;
    }

    public Episode Show()
    {
        IsVisible = true;
        this.AddDomainEventIfAbsent(new EpisodeUpdateEvent(this));
        return this;
    }

    public override void SoftDelete()
    {
        base.SoftDelete();
        AddDomainEvent(new EpisodeDeleteEvent(this.Id));
    }

    public Episode ChangeSubTitle(string subtitleType, string subtitle)
    {
        var parser = SubtitleFactory.GetParser(subtitleType);
        if (parser == null)
        {
            throw new ArgumentOutOfRangeException($"subtitle={subtitleType} is not supported", nameof(subtitleType));
        }
        this.SubtitleType = subtitleType;
        this.SubTitle = subtitle;
        this.AddDomainEventIfAbsent(new EpisodeUpdateEvent(this));
        return this;
    }

    public IEnumerable<Sentence> ParseSubtitle()
    {
        var parser = SubtitleFactory.GetParser(this.SubtitleType);
        return parser.Parse(this.SubTitle);
    }

    public class EpisodeBuilder
    {
        private string id;
        private int sequnceNumber;
        private string name;
        private string albumId;
        private Uri audioUrl;
        private double durationInSecond;
        private string subtitle;
        private string subtitleType;

        public EpisodeBuilder Id(string id)
        {
            this.id = id;
            return this;
        }
        public EpisodeBuilder SequenceNumber(int value)
        {
            this.sequnceNumber = value;
            return this;
        }
        public EpisodeBuilder Name(string value)
        {
            this.name = value;
            return this;
        }
        public EpisodeBuilder AlbumId(string value)
        {
            this.albumId = value;
            return this;
        }
        public EpisodeBuilder AudioUrl(Uri value)
        {
            this.audioUrl = value;
            return this;
        }
        public EpisodeBuilder DurationInSecond(double value)
        {
            this.durationInSecond = value;
            return this;
        }
        public EpisodeBuilder Subtitle(string value)
        {
            this.subtitle = value;
            return this;
        }
        public EpisodeBuilder SubtitleType(string value)
        {
            this.subtitleType = value;
            return this;
        }
        public Episode Build()
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));

            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrEmpty(albumId))
            {
                throw new ArgumentNullException(nameof(albumId));
            }
            if (audioUrl == null)
            {
                throw new ArgumentNullException(nameof(audioUrl));
            }
            if (string.IsNullOrEmpty(subtitle))
            {
                throw new ArgumentNullException(nameof(subtitle));
            }
            if (string.IsNullOrEmpty(subtitleType))
            {
                throw new ArgumentNullException(nameof(subtitleType));
            }
            if (SubtitleFactory.GetParser(subtitleType) == null)
            {
                throw new ArgumentOutOfRangeException($"subtitle={subtitleType} is not supported", nameof(subtitleType));
            }
            if (durationInSecond <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(durationInSecond));
            }
            var episode = new Episode()
            {
                Id = this.id,
                SequenceNumber = this.sequnceNumber,
                Name = this.name,
                AlbumId = this.albumId,
                AudioUrl = this.audioUrl,
                DurationInSecond = this.durationInSecond,
                SubTitle = this.subtitle,
                SubtitleType = this.subtitleType,
                IsVisible = true
            };
            episode.AddDomainEvent(new EpisodeCreateEvent(episode));
            return episode;
        }
    }

}
