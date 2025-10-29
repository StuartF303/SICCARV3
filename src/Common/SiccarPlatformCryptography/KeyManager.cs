// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

// KeyManager Implementation - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Sodium;
#nullable enable

namespace Siccar.Platform.Cryptography
{
	public class KeyManager
	{
		private readonly CryptoModule module;

/*****************************\
* Mnemonic list of 2048 words *
\*****************************/
		private static readonly string[] mnemonic_list = [
			"abandon", "ability", "able", "about", "above", "absent", "absorb", "abstract", "absurd", "abuse",
			"access", "accident", "account", "accuse", "achieve", "acid", "acoustic", "acquire", "across", "act",
			"action", "actor", "actress", "actual", "adapt", "add", "addict", "address", "adjust", "admit",
			"adult", "advance", "advice", "aerobic", "affair", "afford", "afraid", "again", "age", "agent",
			"agree", "ahead", "aim", "air", "airport", "aisle", "alarm", "album", "alcohol", "alert",
			"alien", "all", "alley", "allow", "almost", "alone", "alpha", "already", "also", "alter",
			"always", "amateur", "amazing", "among", "amount", "amused", "analyst", "anchor", "ancient", "anger",
			"angle", "angry", "animal", "ankle", "announce", "annual", "another", "answer", "antenna", "antique",
			"anxiety", "any", "apart", "apology", "appear", "apple", "approve", "april", "arch", "arctic",
			"area", "arena", "argue", "arm", "armed", "armor", "army", "around", "arrange", "arrest",
			"arrive", "arrow", "art", "artefact", "artist", "artwork", "ask", "aspect", "assault", "asset",
			"assist", "assume", "asthma", "athlete", "atom", "attack", "attend", "attitude", "attract", "auction",
			"audit", "august", "aunt", "author", "auto", "autumn", "average", "avocado", "avoid", "awake",
			"aware", "away", "awesome", "awful", "awkward", "axis", "baby", "bachelor", "bacon", "badge",
			"bag", "balance", "balcony", "ball", "bamboo", "banana", "banner", "bar", "barely", "bargain",
			"barrel", "base", "basic", "basket", "battle", "beach", "bean", "beauty", "because", "become",
			"beef", "before", "begin", "behave", "behind", "believe", "below", "belt", "bench", "benefit",
			"best", "betray", "better", "between", "beyond", "bicycle", "bid", "bike", "bind", "biology",
			"bird", "birth", "bitter", "black", "blade", "blame", "blanket", "blast", "bleak", "bless",
			"blind", "blood", "blossom", "blouse", "blue", "blur", "blush", "board", "boat", "body",
			"boil", "bomb", "bone", "bonus", "book", "boost", "border", "boring", "borrow", "boss",
			"bottom", "bounce", "box", "boy", "bracket", "brain", "brand", "brass", "brave", "bread",
			"breeze", "brick", "bridge", "brief", "bright", "bring", "brisk", "broccoli", "broken", "bronze",
			"broom", "brother", "brown", "brush", "bubble", "buddy", "budget", "buffalo", "build", "bulb",
			"bulk", "bullet", "bundle", "bunker", "burden", "burger", "burst", "bus", "business", "busy",
			"butter", "buyer", "buzz", "cabbage", "cabin", "cable", "cactus", "cage", "cake", "call",
			"calm", "camera", "camp", "can", "canal", "cancel", "candy", "cannon", "canoe", "canvas",
			"canyon", "capable", "capital", "captain", "car", "carbon", "card", "cargo", "carpet", "carry",
			"cart", "case", "cash", "casino", "castle", "casual", "cat", "catalog", "catch", "category",
			"cattle", "caught", "cause", "caution", "cave", "ceiling", "celery", "cement", "census", "century",
			"cereal", "certain", "chair", "chalk", "champion", "change", "chaos", "chapter", "charge", "chase",
			"chat", "cheap", "check", "cheese", "chef", "cherry", "chest", "chicken", "chief", "child",
			"chimney", "choice", "choose", "chronic", "chuckle", "chunk", "churn", "cigar", "cinnamon", "circle",
			"citizen", "city", "civil", "claim", "clap", "clarify", "claw", "clay", "clean", "clerk",
			"clever", "click", "client", "cliff", "climb", "clinic", "clip", "clock", "clog", "close",
			"cloth", "cloud", "clown", "club", "clump", "cluster", "clutch", "coach", "coast", "coconut",
			"code", "coffee", "coil", "coin", "collect", "color", "column", "combine", "come", "comfort",
			"comic", "common", "company", "concert", "conduct", "confirm", "congress", "connect", "consider", "control",
			"convince", "cook", "cool", "copper", "copy", "coral", "core", "corn", "correct", "cost",
			"cotton", "couch", "country", "couple", "course", "cousin", "cover", "coyote", "crack", "cradle",
			"craft", "cram", "crane", "crash", "crater", "crawl", "crazy", "cream", "credit", "creek",
			"crew", "cricket", "crime", "crisp", "critic", "crop", "cross", "crouch", "crowd", "crucial",
			"cruel", "cruise", "crumble", "crunch", "crush", "cry", "crystal", "cube", "culture", "cup",
			"cupboard", "curious", "current", "curtain", "curve", "cushion", "custom", "cute", "cycle", "dad",
			"damage", "damp", "dance", "danger", "daring", "dash", "daughter", "dawn", "day", "deal",
			"debate", "debris", "decade", "december", "decide", "decline", "decorate", "decrease", "deer", "defense",
			"define", "defy", "degree", "delay", "deliver", "demand", "demise", "denial", "dentist", "deny",
			"depart", "depend", "deposit", "depth", "deputy", "derive", "describe", "desert", "design", "desk",
			"despair", "destroy", "detail", "detect", "develop", "device", "devote", "diagram", "dial", "diamond",
			"diary", "dice", "diesel", "diet", "differ", "digital", "dignity", "dilemma", "dinner", "dinosaur",
			"direct", "dirt", "disagree", "discover", "disease", "dish", "dismiss", "disorder", "display", "distance",
			"divert", "divide", "divorce", "dizzy", "doctor", "document", "dog", "doll", "dolphin", "domain",
			"donate", "donkey", "donor", "door", "dose", "double", "dove", "draft", "dragon", "drama",
			"drastic", "draw", "dream", "dress", "drift", "drill", "drink", "drip", "drive", "drop",
			"drum", "dry", "duck", "dumb", "dune", "during", "dust", "dutch", "duty", "dwarf",
			"dynamic", "eager", "eagle", "early", "earn", "earth", "easily", "east", "easy", "echo",
			"ecology", "economy", "edge", "edit", "educate", "effort", "egg", "eight", "either", "elbow",
			"elder", "electric", "elegant", "element", "elephant", "elevator", "elite", "else", "embark", "embody",
			"embrace", "emerge", "emotion", "employ", "empower", "empty", "enable", "enact", "end", "endless",
			"endorse", "enemy", "energy", "enforce", "engage", "engine", "enhance", "enjoy", "enlist", "enough",
			"enrich", "enroll", "ensure", "enter", "entire", "entry", "envelope", "episode", "equal", "equip",
			"era", "erase", "erode", "erosion", "error", "erupt", "escape", "essay", "essence", "estate",
			"eternal", "ethics", "evidence", "evil", "evoke", "evolve", "exact", "example", "excess", "exchange",
			"excite", "exclude", "excuse", "execute", "exercise", "exhaust", "exhibit", "exile", "exist", "exit",
			"exotic", "expand", "expect", "expire", "explain", "expose", "express", "extend", "extra", "eye",
			"eyebrow", "fabric", "face", "faculty", "fade", "faint", "faith", "fall", "false", "fame",
			"family", "famous", "fan", "fancy", "fantasy", "farm", "fashion", "fat", "fatal", "father",
			"fatigue", "fault", "favorite", "feature", "february", "federal", "fee", "feed", "feel", "female",
			"fence", "festival", "fetch", "fever", "few", "fiber", "fiction", "field", "figure", "file",
			"film", "filter", "final", "find", "fine", "finger", "finish", "fire", "firm", "first",
			"fiscal", "fish", "fit", "fitness", "fix", "flag", "flame", "flash", "flat", "flavor",
			"flee", "flight", "flip", "float", "flock", "floor", "flower", "fluid", "flush", "fly",
			"foam", "focus", "fog", "foil", "fold", "follow", "food", "foot", "force", "forest",
			"forget", "fork", "fortune", "forum", "forward", "fossil", "foster", "found", "fox", "fragile",
			"frame", "frequent", "fresh", "friend", "fringe", "frog", "front", "frost", "frown", "frozen",
			"fruit", "fuel", "fun", "funny", "furnace", "fury", "future", "gadget", "gain", "galaxy",
			"gallery", "game", "gap", "garage", "garbage", "garden", "garlic", "garment", "gas", "gasp",
			"gate", "gather", "gauge", "gaze", "general", "genius", "genre", "gentle", "genuine", "gesture",
			"ghost", "giant", "gift", "giggle", "ginger", "giraffe", "girl", "give", "glad", "glance",
			"glare", "glass", "glide", "glimpse", "globe", "gloom", "glory", "glove", "glow", "glue",
			"goat", "goddess", "gold", "good", "goose", "gorilla", "gospel", "gossip", "govern", "gown",
			"grab", "grace", "grain", "grant", "grape", "grass", "gravity", "great", "green", "grid",
			"grief", "grit", "grocery", "group", "grow", "grunt", "guard", "guess", "guide", "guilt",
			"guitar", "gun", "gym", "habit", "hair", "half", "hammer", "hamster", "hand", "happy",
			"harbor", "hard", "harsh", "harvest", "hat", "have", "hawk", "hazard", "head", "health",
			"heart", "heavy", "hedgehog", "height", "hello", "helmet", "help", "hen", "hero", "hidden",
			"high", "hill", "hint", "hip", "hire", "history", "hobby", "hockey", "hold", "hole",
			"holiday", "hollow", "home", "honey", "hood", "hope", "horn", "horror", "horse", "hospital",
			"host", "hotel", "hour", "hover", "hub", "huge", "human", "humble", "humor", "hundred",
			"hungry", "hunt", "hurdle", "hurry", "hurt", "husband", "hybrid", "ice", "icon", "idea",
			"identify", "idle", "ignore", "ill", "illegal", "illness", "image", "imitate", "immense", "immune",
			"impact", "impose", "improve", "impulse", "inch", "include", "income", "increase", "index", "indicate",
			"indoor", "industry", "infant", "inflict", "inform", "inhale", "inherit", "initial", "inject", "injury",
			"inmate", "inner", "innocent", "input", "inquiry", "insane", "insect", "inside", "inspire", "install",
			"intact", "interest", "into", "invest", "invite", "involve", "iron", "island", "isolate", "issue",
			"item", "ivory", "jacket", "jaguar", "jar", "jazz", "jealous", "jeans", "jelly", "jewel",
			"job", "join", "joke", "journey", "joy", "judge", "juice", "jump", "jungle", "junior",
			"junk", "just", "kangaroo", "keen", "keep", "ketchup", "key", "kick", "kid", "kidney",
			"kind", "kingdom", "kiss", "kit", "kitchen", "kite", "kitten", "kiwi", "knee", "knife",
			"knock", "know", "lab", "label", "labor", "ladder", "lady", "lake", "lamp", "language",
			"laptop", "large", "later", "latin", "laugh", "laundry", "lava", "law", "lawn", "lawsuit",
			"layer", "lazy", "leader", "leaf", "learn", "leave", "lecture", "left", "leg", "legal",
			"legend", "leisure", "lemon", "lend", "length", "lens", "leopard", "lesson", "letter", "level",
			"liar", "liberty", "library", "license", "life", "lift", "light", "like", "limb", "limit",
			"link", "lion", "liquid", "list", "little", "live", "lizard", "load", "loan", "lobster",
			"local", "lock", "logic", "lonely", "long", "loop", "lottery", "loud", "lounge", "love",
			"loyal", "lucky", "luggage", "lumber", "lunar", "lunch", "luxury", "lyrics", "machine", "mad",
			"magic", "magnet", "maid", "mail", "main", "major", "make", "mammal", "man", "manage",
			"mandate", "mango", "mansion", "manual", "maple", "marble", "march", "margin", "marine", "market",
			"marriage", "mask", "mass", "master", "match", "material", "math", "matrix", "matter", "maximum",
			"maze", "meadow", "mean", "measure", "meat", "mechanic", "medal", "media", "melody", "melt",
			"member", "memory", "mention", "menu", "mercy", "merge", "merit", "merry", "mesh", "message",
			"metal", "method", "middle", "midnight", "milk", "million", "mimic", "mind", "minimum", "minor",
			"minute", "miracle", "mirror", "misery", "miss", "mistake", "mix", "mixed", "mixture", "mobile",
			"model", "modify", "mom", "moment", "monitor", "monkey", "monster", "month", "moon", "moral",
			"more", "morning", "mosquito", "mother", "motion", "motor", "mountain", "mouse", "move", "movie",
			"much", "muffin", "mule", "multiply", "muscle", "museum", "mushroom", "music", "must", "mutual",
			"myself", "mystery", "myth", "naive", "name", "napkin", "narrow", "nasty", "nation", "nature",
			"near", "neck", "need", "negative", "neglect", "neither", "nephew", "nerve", "nest", "net",
			"network", "neutral", "never", "news", "next", "nice", "night", "noble", "noise", "nominee",
			"noodle", "normal", "north", "nose", "notable", "note", "nothing", "notice", "novel", "now",
			"nuclear", "number", "nurse", "nut", "oak", "obey", "object", "oblige", "obscure", "observe",
			"obtain", "obvious", "occur", "ocean", "october", "odor", "off", "offer", "office", "often",
			"oil", "okay", "old", "olive", "olympic", "omit", "once", "one", "onion", "online",
			"only", "open", "opera", "opinion", "oppose", "option", "orange", "orbit", "orchard", "order",
			"ordinary", "organ", "orient", "original", "orphan", "ostrich", "other", "outdoor", "outer", "output",
			"outside", "oval", "oven", "over", "own", "owner", "oxygen", "oyster", "ozone", "pact",
			"paddle", "page", "pair", "palace", "palm", "panda", "panel", "panic", "panther", "paper",
			"parade", "parent", "park", "parrot", "party", "pass", "patch", "path", "patient", "patrol",
			"pattern", "pause", "pave", "payment", "peace", "peanut", "pear", "peasant", "pelican", "pen",
			"penalty", "pencil", "people", "pepper", "perfect", "permit", "person", "pet", "phone", "photo",
			"phrase", "physical", "piano", "picnic", "picture", "piece", "pig", "pigeon", "pill", "pilot",
			"pink", "pioneer", "pipe", "pistol", "pitch", "pizza", "place", "planet", "plastic", "plate",
			"play", "please", "pledge", "pluck", "plug", "plunge", "poem", "poet", "point", "polar",
			"pole", "police", "pond", "pony", "pool", "popular", "portion", "position", "possible", "post",
			"potato", "pottery", "poverty", "powder", "power", "practice", "praise", "predict", "prefer", "prepare",
			"present", "pretty", "prevent", "price", "pride", "primary", "print", "priority", "prison", "private",
			"prize", "problem", "process", "produce", "profit", "program", "project", "promote", "proof", "property",
			"prosper", "protect", "proud", "provide", "public", "pudding", "pull", "pulp", "pulse", "pumpkin",
			"punch", "pupil", "puppy", "purchase", "purity", "purpose", "purse", "push", "put", "puzzle",
			"pyramid", "quality", "quantum", "quarter", "question", "quick", "quit", "quiz", "quote", "rabbit",
			"raccoon", "race", "rack", "radar", "radio", "rail", "rain", "raise", "rally", "ramp",
			"ranch", "random", "range", "rapid", "rare", "rate", "rather", "raven", "raw", "razor",
			"ready", "real", "reason", "rebel", "rebuild", "recall", "receive", "recipe", "record", "recycle",
			"reduce", "reflect", "reform", "refuse", "region", "regret", "regular", "reject", "relax", "release",
			"relief", "rely", "remain", "remember", "remind", "remove", "render", "renew", "rent", "reopen",
			"repair", "repeat", "replace", "report", "require", "rescue", "resemble", "resist", "resource", "response",
			"result", "retire", "retreat", "return", "reunion", "reveal", "review", "reward", "rhythm", "rib",
			"ribbon", "rice", "rich", "ride", "ridge", "rifle", "right", "rigid", "ring", "riot",
			"ripple", "risk", "ritual", "rival", "river", "road", "roast", "robot", "robust", "rocket",
			"romance", "roof", "rookie", "room", "rose", "rotate", "rough", "round", "route", "royal",
			"rubber", "rude", "rug", "rule", "run", "runway", "rural", "sad", "saddle", "sadness",
			"safe", "sail", "salad", "salmon", "salon", "salt", "salute", "same", "sample", "sand",
			"satisfy", "satoshi", "sauce", "sausage", "save", "say", "scale", "scan", "scare", "scatter",
			"scene", "scheme", "school", "science", "scissors", "scorpion", "scout", "scrap", "screen", "script",
			"scrub", "sea", "search", "season", "seat", "second", "secret", "section", "security", "seed",
			"seek", "segment", "select", "sell", "seminar", "senior", "sense", "sentence", "series", "service",
			"session", "settle", "setup", "seven", "shadow", "shaft", "shallow", "share", "shed", "shell",
			"sheriff", "shield", "shift", "shine", "ship", "shiver", "shock", "shoe", "shoot", "shop",
			"short", "shoulder", "shove", "shrimp", "shrug", "shuffle", "shy", "sibling", "sick", "side",
			"siege", "sight", "sign", "silent", "silk", "silly", "silver", "similar", "simple", "since",
			"sing", "siren", "sister", "situate", "six", "size", "skate", "sketch", "ski", "skill",
			"skin", "skirt", "skull", "slab", "slam", "sleep", "slender", "slice", "slide", "slight",
			"slim", "slogan", "slot", "slow", "slush", "small", "smart", "smile", "smoke", "smooth",
			"snack", "snake", "snap", "sniff", "snow", "soap", "soccer", "social", "sock", "soda",
			"soft", "solar", "soldier", "solid", "solution", "solve", "someone", "song", "soon", "sorry",
			"sort", "soul", "sound", "soup", "source", "south", "space", "spare", "spatial", "spawn",
			"speak", "special", "speed", "spell", "spend", "sphere", "spice", "spider", "spike", "spin",
			"spirit", "split", "spoil", "sponsor", "spoon", "sport", "spot", "spray", "spread", "spring",
			"spy", "square", "squeeze", "squirrel", "stable", "stadium", "staff", "stage", "stairs", "stamp",
			"stand", "start", "state", "stay", "steak", "steel", "stem", "step", "stereo", "stick",
			"still", "sting", "stock", "stomach", "stone", "stool", "story", "stove", "strategy", "street",
			"strike", "strong", "struggle", "student", "stuff", "stumble", "style", "subject", "submit", "subway",
			"success", "such", "sudden", "suffer", "sugar", "suggest", "suit", "summer", "sun", "sunny",
			"sunset", "super", "supply", "supreme", "sure", "surface", "surge", "surprise", "surround", "survey",
			"suspect", "sustain", "swallow", "swamp", "swap", "swarm", "swear", "sweet", "swift", "swim",
			"swing", "switch", "sword", "symbol", "symptom", "syrup", "system", "table", "tackle", "tag",
			"tail", "talent", "talk", "tank", "tape", "target", "task", "taste", "tattoo", "taxi",
			"teach", "team", "tell", "ten", "tenant", "tennis", "tent", "term", "test", "text",
			"thank", "that", "theme", "then", "theory", "there", "they", "thing", "this", "thought",
			"three", "thrive", "throw", "thumb", "thunder", "ticket", "tide", "tiger", "tilt", "timber",
			"time", "tiny", "tip", "tired", "tissue", "title", "toast", "tobacco", "today", "toddler",
			"toe", "together", "toilet", "token", "tomato", "tomorrow", "tone", "tongue", "tonight", "tool",
			"tooth", "top", "topic", "topple", "torch", "tornado", "tortoise", "toss", "total", "tourist",
			"toward", "tower", "town", "toy", "track", "trade", "traffic", "tragic", "train", "transfer",
			"trap", "trash", "travel", "tray", "treat", "tree", "trend", "trial", "tribe", "trick",
			"trigger", "trim", "trip", "trophy", "trouble", "truck", "true", "truly", "trumpet", "trust",
			"truth", "try", "tube", "tuition", "tumble", "tuna", "tunnel", "turkey", "turn", "turtle",
			"twelve", "twenty", "twice", "twin", "twist", "two", "type", "typical", "ugly", "umbrella",
			"unable", "unaware", "uncle", "uncover", "under", "undo", "unfair", "unfold", "unhappy", "uniform",
			"unique", "unit", "universe", "unknown", "unlock", "until", "unusual", "unveil", "update", "upgrade",
			"uphold", "upon", "upper", "upset", "urban", "urge", "usage", "use", "used", "useful",
			"useless", "usual", "utility", "vacant", "vacuum", "vague", "valid", "valley", "valve", "van",
			"vanish", "vapor", "various", "vast", "vault", "vehicle", "velvet", "vendor", "venture", "venue",
			"verb", "verify", "version", "very", "vessel", "veteran", "viable", "vibrant", "vicious", "victory",
			"video", "view", "village", "vintage", "violin", "virtual", "virus", "visa", "visit", "visual",
			"vital", "vivid", "vocal", "voice", "void", "volcano", "volume", "vote", "voyage", "wage",
			"wagon", "wait", "walk", "wall", "walnut", "want", "warfare", "warm", "warrior", "wash",
			"wasp", "waste", "water", "wave", "way", "wealth", "weapon", "wear", "weasel", "weather",
			"web", "wedding", "weekend", "weird", "welcome", "west", "wet", "whale", "what", "wheat",
			"wheel", "when", "where", "whip", "whisper", "wide", "width", "wife", "wild", "will",
			"win", "window", "wine", "wing", "wink", "winner", "winter", "wire", "wisdom", "wise",
			"wish", "witness", "wolf", "woman", "wonder", "wood", "wool", "word", "work", "world",
			"worry", "worth", "wrap", "wreck", "wrestle", "wrist", "write", "wrong", "yard", "year",
			"yellow", "you", "young", "youth", "zebra", "zero", "zone", "zoo" ];

/*******************************************************************************\
* KeyChain																		*
* A class used to contain a collection of keyrings. KeyRings can be added or	*
* removed from a KeyChain and a complete KeyChain may be exported or imported	*
* using a supplied password. Exported KeyChains will be compressed and			*
* encrypted. For import of a KeyChain, a CryptoModule may be optionally			*
* supplied for calculation of public keys for the keyrings. Public keys are not	*
* exported as part of the KeyChain.												*
\*******************************************************************************/
		public sealed class KeyChain
		{
			private readonly Dictionary<string, KeyRing> chain = [];
			public (Status status, KeyRing? keyring) GetKeyRing(string name)
			{ 
				return chain.Count > 0 ? (chain.TryGetValue(name, out KeyRing? value) ? (Status.STATUS_OK, value) : (Status.KM_UNKNOWN_KEYRING, null)) : (Status.KM_EMPTY_KEYCHAIN, null);
			}
			public Status AddKeyRing(string name, KeyRing ring)
			{
				if (name == null || name.Length < 1 || ring == null)
					return Status.KM_BAD_KEYRING;
				if (chain.ContainsKey(name))
					return Status.KM_DUPLICATE_KEYRING;
				chain[name] = ring;
				return Status.STATUS_OK;
			}
			public Status RemoveKeyRing(string name)
			{
				return (name != null && name.Length > 0) ? (chain.Count > 0 ? (chain.Remove(name) ? Status.STATUS_OK : Status.KM_UNKNOWN_KEYRING) : Status.KM_EMPTY_KEYCHAIN) : Status.KM_BAD_KEYRING;
			}
			public (Status status, byte[]? chain) Export(string password)
			{
				static List<byte> AddEntry(List<byte> e, List<byte> d)
				{
					d.AddRange(WalletUtils.VLEncode(e.Count));
					d.AddRange(e);
					e.Clear();
					return e;
				}
				if (password == null || password.Length < 1)
					return (Status.KM_PASSWORD_FAIL, null);
				if (chain.Count < 1)
					return (Status.KM_EMPTY_KEYCHAIN, null);
				List<byte> stream = [.. WalletUtils.VLEncode(chain.Count)], entry = [];
				foreach (KeyValuePair<string, KeyRing> ring in chain)
				{
					entry.AddRange(Encoding.ASCII.GetBytes(ring.Key));
					AddEntry(entry, stream).Add((byte)ring.Value.KeySet().PrivateKey.Network);
					entry.AddRange(WalletUtils.VLEncode(ring.Value.KeySet().PrivateKey.Key!.Length));
					entry.AddRange(ring.Value.KeySet().PrivateKey.Key!);
					AddEntry(entry, stream).AddRange(Encoding.ASCII.GetBytes(ring.Value.Mnemonic()));
					AddEntry(entry, stream);
				}
				try
				{
					byte[]? data = WalletUtils.Compress([.. stream], CompressionType.Max).Data;
					if (data != null && data.Length > 0)
					{
						stream.Clear();
						byte[] ks = PasswordHash.ArgonGenerateSalt();
						entry.AddRange(ks);
						byte[] khs = PasswordHash.ArgonGenerateSalt();
						AddEntry(entry, stream).AddRange(khs);
						byte[] k = PasswordHash.ArgonHashBinary(Encoding.ASCII.GetBytes(password), ks, PasswordHash.StrengthArgon.Sensitive, 32, PasswordHash.ArgonAlgorithm.Argon_2ID13);
						AddEntry(entry, stream).AddRange(PasswordHash.ArgonHashBinary(k, khs, PasswordHash.StrengthArgon.Medium, 32, PasswordHash.ArgonAlgorithm.Argon_2ID13));
						byte[] ds = SecretAeadXChaCha20Poly1305.GenerateNonce();
						AddEntry(entry, stream).AddRange(ds);
						AddEntry(entry, stream).AddRange(SecretAeadXChaCha20Poly1305.Encrypt(data, ds, k));
						AddEntry(entry, stream);
						return (Status.STATUS_OK, stream.ToArray());
					}
				}
				catch (Exception) {}
				return (Status.KM_CRYPTO_FAILURE, null);
			}
			public Status Import(byte[] data, string password, CryptoModule? module = null)
			{
				static bool ByteArrayCompare(byte[]? a, byte[]? b) { return a?.Length == b?.Length && ((ReadOnlySpan<byte>)a).SequenceEqual(b); }
				if (password == null || password.Length < 1)
					return Status.KM_PASSWORD_FAIL;
				else if (data == null || data.Length < 1)
					return Status.KM_CRYPTO_FAILURE;
				chain.Clear();
				BinaryReader reader = new(new MemoryStream(data));
				try
				{
					byte[] k = Sodium.PasswordHash.ArgonHashBinary(Encoding.ASCII.GetBytes(password), WalletUtils.ReadVLArray(reader), PasswordHash.StrengthArgon.Sensitive, 32, PasswordHash.ArgonAlgorithm.Argon_2ID13);
					byte[] khs = WalletUtils.ReadVLArray(reader);
					if (!ByteArrayCompare(Sodium.PasswordHash.ArgonHashBinary(k, khs, PasswordHash.StrengthArgon.Medium, 32, PasswordHash.ArgonAlgorithm.Argon_2ID13), WalletUtils.ReadVLArray(reader)))
						return Status.KM_PASSWORD_FAIL;
					byte[] ds = WalletUtils.ReadVLArray(reader);
					reader = new(new MemoryStream(WalletUtils.Decompress(SecretAeadXChaCha20Poly1305.Decrypt(WalletUtils.ReadVLArray(reader), ds, k))!));
					Int32 count = (Int32)WalletUtils.ReadVLSize(reader);
					for (int i = 0; i < count; i++)
					{
						string ks = Encoding.ASCII.GetString(WalletUtils.ReadVLArray(reader));
						WalletNetworks network = (WalletNetworks)WalletUtils.SkipVLSize(reader).ReadByte();
						byte[] pr = WalletUtils.ReadVLArray(reader);
						chain.Add(ks, new KeyRing(pr, network, Encoding.ASCII.GetString(WalletUtils.ReadVLArray(reader)), module ?? new CryptoModule()));
					}
				}
				catch (Exception) {}
				return Status.STATUS_OK;
			}
		}

/*******************************************************************************\
* KeyRing																		*
* Used to hold complete key information including related WIF, Wallet and		*
* recovery key mnemonics. Mnemonics can be obtained as either a long string or	*
* an array of string tokens. For RSA4096 the recovery key is a PKCS#8 PEM		*
* string. Recovery phrases may be optionally protected with a password.			*
\*******************************************************************************/
		public sealed class KeyRing
		{
			private readonly string mnemonics;
			private readonly WalletUtils.KeySet keyset;
			private readonly string wallet;
			private readonly string wif;

			public KeyRing(byte[] privkey, byte[] pubkey, WalletNetworks network, string mnemonic)
			{
				keyset = new WalletUtils.KeySet()
				{
					PrivateKey = new WalletUtils.CryptoKey(network, privkey),
					PublicKey = new WalletUtils.CryptoKey(network, pubkey)
				};
				wallet = WalletUtils.PubKeyToWallet(keyset.PublicKey.Key, (byte)keyset.PublicKey.Network)!;
				wif = WalletUtils.PrivKeyToWIF(keyset.PrivateKey.Key, (byte)keyset.PrivateKey.Network)!;
				mnemonics = mnemonic;
			}
			public KeyRing(byte[] privkey, WalletNetworks network, string mnemonic, CryptoModule module) : this(privkey, module.CalculatePublicKey((byte)network, privkey).pubkey!, network, mnemonic) {}
			public WalletUtils.KeySet KeySet() { return keyset; }
			public string Mnemonic() { return mnemonics; }
			public string Wallet() { return wallet; }
			public string WIFKey() { return wif; }
			public string[] MnemonicList() { return mnemonics.Split(' '); }
		}

/*******************************************************************************\
* KeyManager Constructors()														*
* Constructs the KeyManager object with a supplied CryptoModule or a default	*
* module if one is not provided.												*
\*******************************************************************************/
		public KeyManager() { module = new CryptoModule(); }
		public KeyManager(CryptoModule? cm) { module = cm ?? new CryptoModule(); }

/*******************************************************************************\
* CreateMasterKeyRing()															*
* This method builds a complete Master KeySet for the user. It generates both	*
* public and private keys for the specified network along with a recovery		*
* string. Public and private keys may be extracted in binary, whilst public		*
* keys may also be extracted as a Wallet address and private keys as a WIF		*
* formatted string. An optional password may be used to secure the recovery key	*
* returned by this function. This does not affect the binary keys. Please note	*
* that key generation using this method is non-deterministic and therefore new	*
* KeySets are generated every time it is called.								*
\*******************************************************************************/
		public (Status status, KeyRing? keyring) CreateMasterKeyRing(WalletNetworks network, string? password = null)
		{
			byte[]? seed = null; 
			switch (network)
			{
				case WalletNetworks.ED25519:
				{
					byte[] hd = new byte[WalletNetworks.ED25519.GetPubKeySizeAttribute() + 2];
					SodiumCore.GetRandomBytes(WalletNetworks.ED25519.GetPubKeySizeAttribute()).CopyTo(hd, 1);
					hd[0] = (byte)WalletNetworks.ED25519;
					hd[^1] = WalletUtils.ComputeSHA256Hash(hd.AsSpan(0, hd.Length - 1))[0];
					string mn = GenerateMnemonics(hd);
					seed = GenerateSeed(mn, 32, password);
					(Status status, WalletUtils.KeySet? set) = module.GenerateKeySet(WalletNetworks.ED25519, ref seed);
					if (status != Status.STATUS_OK || set == null)
						break;
					return (Status.STATUS_OK, new KeyRing(set!.Value.PrivateKey.Key!, set.Value.PublicKey.Key!, set.Value.PrivateKey.Network, mn));
				}
				case WalletNetworks.NISTP256:
				{
					(Status status, WalletUtils.KeySet? set) = module.GenerateKeySet(WalletNetworks.NISTP256, ref seed);
					if (status != Status.STATUS_OK || set == null)
						break;
					byte[]? hd = new byte[set!.Value.PrivateKey.Key!.Length + 2];
					hd[0] = (byte)WalletNetworks.NISTP256;
					set.Value.PrivateKey.Key.CopyTo(hd, 1);
					if (password != null && password.Length > 0)
					{
						byte[] eb = GenerateSeed(password);
						for (int i = 0; i < eb.Length; i++)
							hd[i + 1] ^= eb[i];
					}
					hd[^1] = WalletUtils.ComputeSHA256Hash(hd.AsSpan(0, hd.Length - 1))[0];
					return (Status.STATUS_OK, new KeyRing(set.Value.PrivateKey.Key, set.Value.PublicKey.Key!, set.Value.PrivateKey.Network, GenerateMnemonics(hd)));
				}
				case WalletNetworks.RSA4096:
				{
					if (password != null && password.Length > 0)
						seed = Encoding.ASCII.GetBytes(password!);
					(Status status, WalletUtils.KeySet? set) = module.GenerateKeySet(WalletNetworks.RSA4096, ref seed);
					if (status != Status.STATUS_OK || set == null)
						break;
					return (Status.STATUS_OK, new KeyRing(set.Value.PrivateKey.Key!, set.Value.PublicKey.Key!, set.Value.PrivateKey.Network, Encoding.ASCII.GetString(seed!)));
				}
				default: { break; }
			}
			return (Status.KM_GENERATE_FAIL, null);
		}

/*******************************************************************************\
* RecoverMasterKeyRing()														*
* This method is used to recover a KeySet from a recovery mnemonic string. This	*
* recovery string may be encrypted and a password might be required in order to	*
* complete recovery. The string may also be an PKCS #8 PEM formatted private	*
* key. If recovery fails, then a Status error will be returned and the keyring	*
* will be null.																	*
\*******************************************************************************/
		public (Status status, KeyRing? keyring) RecoverMasterKeyRing(string? mnemonics, string? password = null)
		{
			byte[][] ids = [
				[
					0xe7, 0x1b, 0x53, 0xa7, 0x11, 0x5f, 0x6f, 0xea, 0xd8, 0xa6, 0x49, 0xcf, 0x75, 0x97, 0xc1, 0xc1,
					0x01, 0x55, 0x46, 0x21, 0xd7, 0x40, 0xd7, 0x10, 0x67, 0x9c, 0xd8, 0xa5, 0x4a, 0x1b, 0x12, 0xb6 ],
				[
					0xe3, 0x21, 0x22, 0x6c, 0x65, 0xca, 0xae, 0x02, 0x56, 0x1e, 0xfc, 0xba, 0x06, 0xf5, 0x1c, 0xc2,
					0xa1, 0x53, 0x9a, 0xa4, 0x74, 0x7e, 0x7b, 0x23, 0x66, 0x26, 0xa6, 0x68, 0xa7, 0xfb, 0x05, 0xeb ] ];
			if (mnemonics != null)
			{
				mnemonics = mnemonics.Trim();
				if (mnemonics.Length > 9 && WalletUtils.HashData(Encoding.ASCII.GetBytes(mnemonics[..10]), HashType.Blake2b_256)!.SequenceEqual(ids[0]))
				{
					(Status status, WalletUtils.KeySet? set) = module.RecoverKeySet(WalletNetworks.RSA4096, Encoding.ASCII.GetBytes(mnemonics),
						(mnemonics.Length > 19 && WalletUtils.HashData(Encoding.ASCII.GetBytes(mnemonics[..20]), HashType.Blake2b_256)!.SequenceEqual(ids[1])) ? password : null);
					if (status == Status.KM_PASSWORD_FAIL)
						return (status, null);
					else if (status == Status.STATUS_OK && set != null)
						return (status, new KeyRing(set.Value.PrivateKey.Key!, set.Value.PublicKey.Key!, set.Value.PrivateKey.Network, mnemonics));
				}
				else
				{
					string[] ml = mnemonics.ToLower().Split(' ');
					if (ml.Length > 11)
					{
						byte[]? bits = DecodeMnemonics(ml);
						if (bits != null && Enum.IsDefined(typeof(WalletNetworks), bits[0]) && WalletUtils.ComputeSHA256Hash(bits.AsSpan(0, bits.Length - 1))[0] == bits[^1])
						{
							switch ((WalletNetworks)bits[0])
							{
								case WalletNetworks.NISTP256:
								{
									if (password != null && password.Length > 0)
									{
										byte[] eb = GenerateSeed(password);
										for (int i = 0; i < eb.Length; i++)
											bits[i + 1] ^= eb[i];
									}
									(Status status, WalletUtils.KeySet? set) = module.RecoverKeySet(WalletNetworks.NISTP256, bits);
									if (status != Status.STATUS_OK || set == null)
										break;
									return (Status.STATUS_OK, new KeyRing(set.Value.PrivateKey.Key!, set.Value.PublicKey.Key!, WalletNetworks.NISTP256, mnemonics));
								}
								case WalletNetworks.ED25519:
								{
									(Status status, WalletUtils.KeySet? set) = module.RecoverKeySet(WalletNetworks.ED25519, GenerateSeed(mnemonics, 32, password));
									if (status != Status.STATUS_OK || set == null)
										break;
									return (Status.STATUS_OK, new KeyRing(set.Value.PrivateKey.Key!, set.Value.PublicKey.Key!, WalletNetworks.ED25519, mnemonics));
								}
								default: { break; }
							}
						}
					}
				}
			}
			return (Status.KM_BAD_MNEMONIC, null);
		}

/*******************************************************************************\
* GenerateMnemonics()															*
* This internal method is used to generate a string of mnemonics from a			*
* provided input byte array. The array may be of any arbitrary length and will	*
* be padded to fit mnemonic conversion.											*
\*******************************************************************************/
		private static string GenerateMnemonics(byte[] data)
		{
			StringBuilder builder = new();
			for (int i = 0; i < (data.Length / 11); i++)
			{
				builder.Append(mnemonic_list[(data[0 + (i * 11)] << 3) | (data[1 + (i * 11)] >> 5)]).Append(' ');
				builder.Append(mnemonic_list[((data[1 + (i * 11)] & 0x1f) << 6) | (data[2 + (i * 11)] >> 2)]).Append(' ');
				builder.Append(mnemonic_list[((data[2 + (i * 11)] & 0x03) << 9) | (data[3 + (i * 11)] << 1) | (data[4 + (i * 11)] >> 7)]).Append(' ');
				builder.Append(mnemonic_list[((data[4 + (i * 11)] & 0x7f) << 4) | (data[5 + (i * 11)] >> 4)]).Append(' ');
				builder.Append(mnemonic_list[((data[5 + (i * 11)] & 0x0f) << 7) | (data[6 + (i * 11)] >> 1)]).Append(' ');
				builder.Append(mnemonic_list[((data[6 + (i * 11)] & 0x01) << 10) | (data[7 + (i * 11)] << 2) | (data[8 + (i * 11)] >> 6)]).Append(' ');
				builder.Append(mnemonic_list[((data[8 + (i * 11)] & 0x3f) << 5) | (data[9 + (i * 11)] >> 3)]).Append(' ');
				builder.Append(mnemonic_list[((data[9 + (i * 11)] & 0x07) << 8) | data[10 + (i * 11)]]).Append(' ');
			}
			UInt32 sz = (UInt32)data.Length % 11;
			if (sz > 0)
			{
				List<string> nms = [];
				byte[] ovr = new byte[sz + 2];
				data.AsSpan(data.Length - (int)sz, (int)sz).CopyTo(ovr);
				switch ((sz << 3) / 11)
				{
					case 7:
						nms.Insert(0, mnemonic_list[((ovr[9] & 0x07) << 8) | ovr[10]] + ' ');
						goto case 6;
					case 6:
						nms.Insert(0, mnemonic_list[((ovr[8] & 0x3f) << 5) | (ovr[9] >> 3)] + ' ');
						goto case 5;
					case 5:
						nms.Insert(0, mnemonic_list[((ovr[6] & 0x01) << 10) | (ovr[7] << 2) | (ovr[8] >> 6)] + ' ');
						goto case 4;
					case 4:
						nms.Insert(0, mnemonic_list[((ovr[5] & 0x0f) << 7) | (ovr[6] >> 1)] + ' ');
						goto case 3;
					case 3:
						nms.Insert(0, mnemonic_list[((ovr[4] & 0x7f) << 4) | (ovr[5] >> 4)] + ' ');
						goto case 2;
					case 2:
						nms.Insert(0, mnemonic_list[((ovr[2] & 0x03) << 9) | (ovr[3] << 1) | (ovr[4] >> 7)] + ' ');
						goto case 1;
					case 1:
						nms.Insert(0, mnemonic_list[((ovr[1] & 0x1f) << 6) | (ovr[2] >> 2)] + ' ');
						goto case 0;
					case 0:
						nms.Insert(0, mnemonic_list[(ovr[0] << 3) | (ovr[1] >> 5)] + ' ');
						break;
					default: break;
				}
				nms.ForEach(n => builder.Append(n));
			}
			return builder.Remove(builder.Length - 1, 1).ToString();
		}

/*******************************************************************************\
* DecodeMnemonics()																*
* An internal method for turning an array of mnemonics into its corresponding	*
* byte array. This routine is NOT guaranteed to always return a byte array of	*
* the same size that was used to create the list. This is due to the fact that	*
* an input array byte size may not have been evenly	divisible by the mnemonic	*
* size. It will return a byte array containing as many complete bytes as		*
* possible and therefore may be one byte larger than expected for certain		*
* lengths of mnemonic lists.													*
\*******************************************************************************/
		private static byte[]? DecodeMnemonics(string[] list)
		{
			static int FindMnemonic(string w)
			{
				int lb = 0, ub = mnemonic_list.Length - 1, r, m;
				do
				{
					m = lb + ((ub - lb) >> 1);
					if (((ub - lb) >> 1) <= 0)
					{
						m = w.CompareTo(mnemonic_list[lb]) == 0 ? lb : w.CompareTo(mnemonic_list[ub]) == 0 ? ub : -1;
						break;
					}
					r = w.CompareTo(mnemonic_list[m]);
					if (r < 0)
						ub = m;
					else if (r > 0)
						lb = m;
				} while (r != 0);
				return m;
			}

			int[] terms = new int[list.Length];
			for (int i = 0; i < list.Length; i++)
			{
				terms[i] = FindMnemonic(list[i]);
				if (terms[i] == -1)
					return null;
			}
			byte[] data = new byte[(terms.Length * 11) >> 3];
			int j = 0;
			for (; j < data.Length / 11; j++)
			{
				data[0 + (j * 11)] = (byte)(terms[0 + (j * 8)] >> 3);
				data[1 + (j * 11)] = (byte)((terms[0 + (j * 8)] << 5) | (terms[1 + (j * 8)] >> 6));
				data[2 + (j * 11)] = (byte)((terms[1 + (j * 8)] << 2) | (terms[2 + (j * 8)] >> 9));
				data[3 + (j * 11)] = (byte)(terms[2 + (j * 8)] >> 1);
				data[4 + (j * 11)] = (byte)((terms[2 + (j * 8)] << 7) | (terms[3 + (j * 8)] >> 4));
				data[5 + (j * 11)] = (byte)((terms[3 + (j * 8)] << 4) | (terms[4 + (j * 8)] >> 7));
				data[6 + (j * 11)] = (byte)((terms[4 + (j * 8)] << 1) | (terms[5 + (j * 8)] >> 10));
				data[7 + (j * 11)] = (byte)(terms[5 + (j * 8)] >> 2);
				data[8 + (j * 11)] = (byte)((terms[5 + (j * 8)] << 6) | (terms[6 + (j * 8)] >> 5));
				data[9 + (j * 11)] = (byte)((terms[6 + (j * 8)] << 3) | (terms[7 + (j * 8)] >> 8));
				data[10 + (j * 11)] = (byte)terms[7 + (j * 8)];
			}
			UInt32 sz = (UInt32)data.Length % 11;
			if (sz > 0)
			{
				switch (sz - 1)
				{
					case 10:
						data[10 + (j * 11)] = (byte)terms[7 + (j * 8)];
						goto case 9;
					case 9:
						data[9 + (j * 11)] = (byte)((terms[6 + (j * 8)] << 3) | (terms[7 + (j * 8)] >> 8));
						goto case 8;
					case 8:
						data[8 + (j * 11)] = (byte)((terms[5 + (j * 8)] << 6) | (terms[6 + (j * 8)] >> 5));
						goto case 7;
					case 7:
						data[7 + (j * 11)] = (byte)(terms[5 + (j * 8)] >> 2);
						goto case 6;
					case 6:
						data[6 + (j * 11)] = (byte)((terms[4 + (j * 8)] << 1) | (terms[5 + (j * 8)] >> 10));
						goto case 5;
					case 5:
						data[5 + (j * 11)] = (byte)((terms[3 + (j * 8)] << 4) | (terms[4 + (j * 8)] >> 7));
						goto case 4;
					case 4:
						data[4 + (j * 11)] = (byte)((terms[2 + (j * 8)] << 7) | (terms[3 + (j * 8)] >> 4));
						goto case 3;
					case 3:
						data[3 + (j * 11)] = (byte)(terms[2 + (j * 8)] >> 1);
						goto case 2;
					case 2:
						data[2 + (j * 11)] = (byte)((terms[1 + (j * 8)] << 2) | (terms[2 + (j * 8)] >> 9));
						goto case 1;
					case 1:
						data[1 + (j * 11)] = (byte)((terms[0 + (j * 8)] << 5) | (terms[1 + (j * 8)] >> 6));
						goto case 0;
					case 0:
						data[0 + (j * 11)] = (byte)(terms[0 + (j * 8)] >> 3);
						break;
					default: break;
				}
			}
			return data;
		}

/*******************************************************************************\
* GenerateSeed()																*
* An internal function used to securely hash a supplied string. An optional		*
* password maybe used to enhance the supplied input stream to the hash			*
* function.	An optional output byte count may be provided as an input			*
* parameter, but by	default this method returns 32 bytes for any given input.	*
* This function is designed to be particularly slow to prevent brute-force hash	*
* attacks.																		*
\*******************************************************************************/
		private static byte[] GenerateSeed(string mnemonics, UInt32 count = 32, string? password = null)
		{
			password ??= string.Empty;
			string pw = mnemonics.ToLower().Normalize(NormalizationForm.FormKD);
			string salt = Convert.ToBase64String(WalletUtils.ComputeSHA512Hash(Encoding.UTF8.GetBytes(password.Insert(0, "mnemonic")
				.Normalize(NormalizationForm.FormKD)).AsSpan()))[..16];
			return Sodium.PasswordHash.ArgonHashBinary(pw, salt, PasswordHash.StrengthArgon.Sensitive, count, PasswordHash.ArgonAlgorithm.Argon_2ID13);
		}
	}
}
