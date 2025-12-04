#include<stdio.h>
#include<string.h>
#include<stdlib.h>
#include<assert.h>
#include<stdint.h>
#include<immintrin.h>

static void trimnl(char * str) {
	while(*str && *str != '\n') {
		str++;
	}
	*str = '\0';
}

static void zerorow(uint8_t** pdat, int stride)
{
	memset(*pdat, 0, stride);
	*pdat += stride;
}

static void loadrow(uint8_t* dat, char * buf) {
	*dat++ = 0; // left padding
	while(*buf && *buf != '\n') {

		assert(*buf == '.' || *buf == '@');

		if (*buf == '.')
			*dat = 0;
		else if (*buf == '@')
			*dat = 1;

		buf++;
		dat++;
	}
}

static size_t sum(uint8_t* i, size_t len)
{
	size_t ret = 0;
	size_t idx;
	for (idx = 0; idx < len; idx++)
	{
		ret += i[idx];
	}
	return ret;
}

void step1row(const uint8_t* input, uint8_t* output, int stride) {

	__m256i four = _mm256_set1_epi8(4);

	const uint8_t* t = input - stride;
	const uint8_t* m = input;
	const uint8_t* b = input + stride;

	for (int i = 1; i < stride - 1; i += 32) {

		// convolution kernel
		// 1 1 1
		// 1 1 1
		// 1 1 1
		__m256i tl = _mm256_loadu_si256((const __m256i*)(t + i - 1));
		__m256i tc = _mm256_loadu_si256((const __m256i*)(t + i));
		__m256i tr = _mm256_loadu_si256((const __m256i*)(t + i + 1));
		__m256i ts = _mm256_add_epi8(tl, _mm256_add_epi8(tc, tr));

		__m256i ml = _mm256_loadu_si256((const __m256i*)(m + i - 1));
		__m256i mc = _mm256_loadu_si256((const __m256i*)(m + i));
		__m256i mr = _mm256_loadu_si256((const __m256i*)(m + i + 1));
		__m256i ms = _mm256_add_epi8(ml, _mm256_add_epi8(mc, mr));

		__m256i bl = _mm256_loadu_si256((const __m256i*)(b + i - 1));
		__m256i bc = _mm256_loadu_si256((const __m256i*)(b + i));
		__m256i br = _mm256_loadu_si256((const __m256i*)(b + i + 1));
		__m256i bs = _mm256_add_epi8(bl, _mm256_add_epi8(bc, br));

		__m256i sum = _mm256_add_epi8(ts, _mm256_add_epi8(ms, bs));

		// remove middle column -> convo kernel 
		// 1 1 1
		// 1 0 1
		// 1 1 1
		__m256i neighbors = _mm256_sub_epi8(sum, mc);

		// <= 4 neighbors (avx can do only lt, not lte)
		__m256i le_4 = _mm256_cmpgt_epi8(four, neighbors);

		// AND with middle (middle must be 1 for everything to be 1)
		// this effectively reduces it from 0/xff to 0/1
		__m256i result = _mm256_and_si256(le_4, mc);

		_mm256_storeu_si256((__m256i*)(output + i), result);
	}	
}

static void step1(const uint8_t* input, uint8_t* output, int stride) {

	for (int y = 1; y < stride - 1; y++)
	{
		const uint8_t* inrow = input + (y * stride);
		const uint8_t* outrow = output + (y * stride);
		step1row(inrow, outrow, stride);
	}
}

static void writeimgs(const uint8_t* input, const uint8_t* overlay, int stride)
{
	for(int y = 0; y < stride; y++) {
		for(int x = 0; x < stride; x++) {
			uint8_t v = input[y * stride + x];
			uint8_t o = overlay[y * stride + x];

			if (o && v)
				printf("X");
			else if (v)
				printf("@");
			else if (o && !v)
				printf("?");
			else
				printf(".");
		}
		printf("\n");
	}
}

static void removeoverlay(uint8_t* data, const uint8_t* overlay, int stride) {
	// optimize: skip first and last row
	for (int y = 1; y < stride - 1; y++)
	{
		for (int x = 1; x < stride - 1; x += 32)
		{
			int index = y * stride + x;

			__m256i i = _mm256_loadu_si256((const __m256i*)(data + index));
			__m256i r = _mm256_loadu_si256((const __m256i*)(overlay + index));

			__m256i s = _mm256_xor_si256(i, r);
			_mm256_storeu_si256((__m256i*)(data + index), s);
		}
	}
}

static void clear(uint8_t* image, int stride) {
	memset(image, 0, stride * stride * sizeof(uint8_t));
}

int main()
{
	FILE * fp = fopen("input.txt", "rt");
	assert(fp && "Failed to open input");

	char buf[150]; // maxlen

	fgets(buf, sizeof(buf), fp);
	trimnl(buf);

	size_t rows = strlen(buf);
	// round to multiple of 32
	size_t stride = (rows + 2 + 31) & ~31;

	printf("image size: %lld x %lld\n", rows, rows);
	printf("image stride: %lld x %lld\n", stride, stride);


	// pad each row with zeroes at begining and start,
	// so the convolution kernel doesn't need special handling at edges
	uint8_t * image = malloc(stride * stride * sizeof(uint8_t));
	uint8_t* image2 = malloc(stride * stride * sizeof(uint8_t));

	assert(image && image2 && "Failed to allocate images");

	clear(image, stride);

	{
		uint8_t * dat = image;
		zerorow(&dat, stride);
		do {
			loadrow(dat, buf);
			dat += stride;
		} while(fgets(buf, sizeof(buf), fp));
		zerorow(&dat, stride);
	}

	int step = 0;
	size_t removed;
	size_t total = 0;
	do
	{
		clear(image2, stride);
		step1(image, image2, stride);
		removed = sum(image2, stride * stride);

		printf("step %d: %lld, total: %lld\n", step++, removed, total += removed);

		// writeimgs(image, image2, stride);

		removeoverlay(image, image2, stride);
	} while (removed > 0);

	free(image2);
	free(image);
	fclose(fp);
}
