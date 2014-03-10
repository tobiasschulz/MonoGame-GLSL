#!/usr/bin/perl

use strict;
use warnings;

sub find_cs_files {
	my ($dir, $exclude) = @_;

	return
		grep { my $file = $_; scalar (grep { $file =~ /$_/i } @$exclude) ? 0 : 1 }
		map { s/$dir//; $_ } split(/[\r\n]+/, `find $dir -name "*.cs"`);
}

sub format_cs_files {
	my (@files) = @_;
	return map { s/\//\\/gm; '    <Compile Include="'.$_.'" />' } sort @files;
}

sub rewrite_csproj {
	my ($args) = @_;

	open my $f, '<', $args->{csproj};
	my @lines = <$f>;
	map { tr/[\r\n]//d; } @lines;
	close $f;

	my @newlines = ();

	while (my $line = shift @lines) {
		last if $line =~ /Compile Include/;
		push @newlines, $line;
	}
	while (my $line = shift @lines) {
		if ($line !~ /Compile Include/) {
			unshift @lines, $line;
			last;
		}
	}
	push @newlines, format_cs_files(find_cs_files($args->{dir}, $args->{exclude}));
	while (my $line = shift @lines) {
		next if $line =~ /Compile Include/;
		push @newlines, $line;
	}

	open $f, '>', $args->{csproj};
	print $f join($args->{linesep}, @newlines, "");
	close $f;
}

my @csproj_files = (
	{ csproj => 'MonoGame.GLSL/MonoGame.GLSL.csproj', dir => 'MonoGame.GLSL/', exclude => [], linesep => qq[\n] },
	{ csproj => 'Framework/Knot3.Framework-XNA.csproj', dir => 'Framework/', exclude => ['-MG.cs'], linesep => qq[\n] },
	{ csproj => 'UnitTests/Knot3.UnitTests-MG.csproj', dir => 'UnitTests/', exclude => [], linesep => qq[\n] },
	{ csproj => 'UnitTests/Knot3.UnitTests-XNA.csproj', dir => 'UnitTests/', exclude => [], linesep => qq[\n] },
	{ csproj => 'Tools/ConfigReset/Knot3.ConfigReset.csproj', dir => 'Tools/ConfigReset/', exclude => [], linesep => qq[\n] },
	{ csproj => 'ExtremeTests/Knot3.ExtremeTests-MG.csproj', dir => 'ExtremeTests/', exclude => [], linesep => qq[\n] },
	{ csproj => 'ExtremeTests/Knot3.ExtremeTests-XNA.csproj', dir => 'ExtremeTests/', exclude => [], linesep => qq[\n] },
	{ csproj => 'VisualTests/Knot3.VisualTests-MG.csproj', dir => 'VisualTests/', exclude => [], linesep => qq[\n] },
);

foreach my $csproj_file (@csproj_files) {
	rewrite_csproj($csproj_file);
}
